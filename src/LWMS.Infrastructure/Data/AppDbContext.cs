using Microsoft.EntityFrameworkCore;
using LWMS.Domain.Entities;
using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LWMS.Infrastructure.Data
{
    public class AppDbContext : DbContext, IApplicationDbContext
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly Data.Interceptors.AuditInterceptor? _auditInterceptor;

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            ICurrentUserService currentUserService,
            Data.Interceptors.AuditInterceptor? auditInterceptor = null) : base(options)
        {
            _currentUserService = currentUserService;
            _auditInterceptor = auditInterceptor;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_auditInterceptor != null)
            {
                optionsBuilder.AddInterceptors(_auditInterceptor);
            }
            base.OnConfiguring(optionsBuilder);
        }

        // ── DbSets ──
        public DbSet<Parcel> Parcels => Set<Parcel>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Merchant> Merchants => Set<Merchant>();
        public DbSet<Hub> Hubs => Set<Hub>();
        public DbSet<Bag> Bags => Set<Bag>();
        public DbSet<BagItem> BagItems => Set<BagItem>();
        public DbSet<Rack> Racks => Set<Rack>();
        public DbSet<ParcelLocation> ParcelLocations => Set<ParcelLocation>();
        public DbSet<TrackingLog> TrackingLogs => Set<TrackingLog>();
        public DbSet<ShipperAssignment> ShipperAssignments => Set<ShipperAssignment>();
        public DbSet<CodRecord> CodRecords => Set<CodRecord>();
        public DbSet<CodSettlement> CodSettlements => Set<CodSettlement>();
        public DbSet<CodSettlementItem> CodSettlementItems => Set<CodSettlementItem>();
        public DbSet<ReturnOrder> ReturnOrders => Set<ReturnOrder>();
        public DbSet<PickupRequest> PickupRequests => Set<PickupRequest>();
        public DbSet<ServiceType> ServiceTypes => Set<ServiceType>();
        public DbSet<FeeConfig> FeeConfigs => Set<FeeConfig>();
        public DbSet<Zone> Zones => Set<Zone>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ☁️ TỐI ƯU CHO TIDB CLOUD / MYSQL
            modelBuilder.HasCharSet("utf8mb4");
            modelBuilder.UseCollation("utf8mb4_unicode_ci");

            // 💰 COD RECORD - Chặn Double COD
            modelBuilder.Entity<CodRecord>()
                .HasIndex(c => new { c.ParcelId, c.Status })
                .IsUnique();

            // 📦 BAG ITEM - Chặn 1 đơn hàng vào 2 bao tải trong cùng 1 hành trình
            modelBuilder.Entity<BagItem>()
                .HasIndex(b => b.ParcelId)
                .IsUnique(); 

            // 🚀 TỐI ƯU TRUY VẤN (INDEXING)
            modelBuilder.Entity<Parcel>()
                .HasIndex(p => p.TrackingCode)
                .IsUnique();
            
            modelBuilder.Entity<Parcel>()
                .HasIndex(p => new { p.SlaDate, p.Status });

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            // 🛠 ÉP TOÀN BỘ CỘT GUID DÙNG UTF8MB4 (Bỏ rơi ascii_general_ci gây lỗi trên TiDB)
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(Guid) || property.ClrType == typeof(Guid?))
                    {
                        property.SetColumnType("char(36)");
                        property.SetCollation("utf8mb4_unicode_ci");
                    }
                }

                // 1. Soft Delete Filter
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var method = typeof(AppDbContext).GetMethod(nameof(SetSoftDeleteFilter), 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                        .MakeGenericMethod(entityType.ClrType);
                    method.Invoke(this, new object[] { modelBuilder });
                }

                // 2. Tenant Filter (IMustHaveMerchant)
                if (typeof(IMustHaveMerchant).IsAssignableFrom(entityType.ClrType))
                {
                    var method = typeof(AppDbContext).GetMethod(nameof(SetMerchantFilter), 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                        .MakeGenericMethod(entityType.ClrType);
                    method.Invoke(this, new object[] { modelBuilder });
                }
            }
        }

    // ──────────────────────────────────────────────
    // 🛠 DYNAMIC FILTER BUILDERS (EVALUATED AT RUNTIME)
    // ──────────────────────────────────────────────
    private void SetSoftDeleteFilter<T>(ModelBuilder modelBuilder) where T : BaseEntity
    {
        modelBuilder.Entity<T>().HasQueryFilter(x => !x.IsDeleted);
    }

    private void SetMerchantFilter<T>(ModelBuilder modelBuilder) where T : class, IMustHaveMerchant
    {
        modelBuilder.Entity<T>().HasQueryFilter(x => 
            _currentUserService.MerchantId == null || x.MerchantId == _currentUserService.MerchantId);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 1. Capture changes for Cloud Sync (Outbox Pattern)
        var entries = ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Where(e => e.Entity is not OutboxMessage && e.Entity is not AuditLog) // Skip meta entities
            .Where(e => !e.Metadata.IsOwned()) // Skip value objects / owned entities
            .ToList();

        foreach (var entry in entries)
        {
            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                EntityName = entry.Entity.GetType().Name,
                EntityId = entry.Property("Id").CurrentValue?.ToString() ?? "N/A",
                Action = entry.State.ToString(),
                DataJson = System.Text.Json.JsonSerializer.Serialize(entry.Entity, new System.Text.Json.JsonSerializerOptions 
                { 
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                    WriteIndented = false 
                }),
                CreatedAt = DateTime.UtcNow,
                IsProcessed = false
            };
            OutboxMessages.Add(outboxMessage);
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
}
