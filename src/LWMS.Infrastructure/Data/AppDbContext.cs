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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 💰 COD RECORD - Chặn Double COD
            modelBuilder.Entity<CodRecord>()
                .HasIndex(c => new { c.ParcelId, c.Status })
                .IsUnique()
                .HasFilter("[Status] = 'COLLECTED'"); // Chỉ cho phép 1 record COLLECTED mỗi đơn

            // 📦 BAG ITEM - Chặn 1 đơn hàng vào 2 bao tải trong cùng 1 hành trình
            modelBuilder.Entity<BagItem>()
                .HasIndex(b => b.ParcelId)
                .IsUnique(); 

            // 🚀 TỐI ƯU TRUY VẤN (INDEXING)
            modelBuilder.Entity<Parcel>()
                .HasIndex(p => p.TrackingCode)
                .IsUnique();
            
            modelBuilder.Entity<Parcel>()
                .HasIndex(p => new { p.SlaDate, p.Status })
                .HasFilter("[SlaDate] IS NOT NULL");

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            // ── GLOBAL QUERY FILTERS ──
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // 1. Soft Delete Filter
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType).AddSoftDeleteFilter();
                }

                // 2. Tenant Filter (IMustHaveMerchant)
                if (typeof(IMustHaveMerchant).IsAssignableFrom(entityType.ClrType))
                {
                    var merchantId = _currentUserService.MerchantId;
                    if (merchantId.HasValue)
                    {
                        modelBuilder.Entity(entityType.ClrType).AddMerchantFilter(merchantId.Value);
                    }
                }
            }
        }
    }

    // ──────────────────────────────────────────────
    // 🛠 EXTENSION METHODS CHO QUERY FILTER (Tránh dùng MethodInfo.Invoke)
    // ──────────────────────────────────────────────
    public static class ExpressionExtensions
    {
        public static void AddSoftDeleteFilter(this EntityTypeBuilder entityTypeBuilder)
        {
            var method = typeof(ExpressionExtensions).GetMethod(nameof(GetSoftDeleteFilter), 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                .MakeGenericMethod(entityTypeBuilder.Metadata.ClrType);
            var filter = method.Invoke(null, null);
            entityTypeBuilder.HasQueryFilter((dynamic)filter!);
        }

        public static void AddMerchantFilter(this EntityTypeBuilder entityTypeBuilder, Guid merchantId)
        {
            var method = typeof(ExpressionExtensions).GetMethod(nameof(GetMerchantFilter), 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                .MakeGenericMethod(entityTypeBuilder.Metadata.ClrType);
            var filter = method.Invoke(null, new object[] { merchantId });
            entityTypeBuilder.HasQueryFilter((dynamic)filter!);
        }

        private static System.Linq.Expressions.Expression<Func<T, bool>> GetSoftDeleteFilter<T>() where T : BaseEntity
            => x => !x.IsDeleted;

        private static System.Linq.Expressions.Expression<Func<T, bool>> GetMerchantFilter<T>(Guid merchantId) where T : class, IMustHaveMerchant
            => x => x.MerchantId == merchantId;
    }
}