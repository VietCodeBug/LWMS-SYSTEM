using LWMS.Domain.Common;
using LWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LWMS.Infrastructure.Data.Configurations;

// ═══════════════════════════════════════════
// 🔥 PARCEL (ENTITY TRUNG TÂM)
// ═══════════════════════════════════════════
public class ParcelConfiguration : IEntityTypeConfiguration<Parcel>
{
    public void Configure(EntityTypeBuilder<Parcel> builder)
    {
        builder.ToTable("parcels");

        // Relationships
        builder.HasOne<Merchant>().WithMany().HasForeignKey(p => p.MerchantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ServiceType>().WithMany().HasForeignKey(p => p.ServiceTypeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Hub>().WithMany().HasForeignKey(p => p.OriginHubId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Hub>().WithMany().HasForeignKey(p => p.DestHubId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Hub>().WithMany().HasForeignKey(p => p.CurrentHubId).OnDelete(DeleteBehavior.Restrict);

        // Value Objects
        builder.OwnsOne(p => p.CodAmount, pb => {
            pb.Property(m => m.Amount).HasColumnName("cod_amount").HasColumnType("decimal(18,2)");
            pb.Property(m => m.Currency).HasColumnName("cod_currency").HasMaxLength(3);
        });
        builder.OwnsOne(p => p.SenderAddress, ab => {
            ab.Property(a => a.Street).HasColumnName("sender_street");
            ab.Property(a => a.Ward).HasColumnName("sender_ward");
            ab.Property(a => a.District).HasColumnName("sender_district");
            ab.Property(a => a.Province).HasColumnName("sender_province");
        });
        builder.OwnsOne(p => p.ReceiverAddress, ab => {
            ab.Property(a => a.Street).HasColumnName("receiver_street");
            ab.Property(a => a.Ward).HasColumnName("receiver_ward");
            ab.Property(a => a.District).HasColumnName("receiver_district");
            ab.Property(a => a.Province).HasColumnName("receiver_province");
        });

        builder.Property(p => p.Weight).HasColumnType("decimal(18,2)");
        builder.Property(p => p.RowVersion).IsRowVersion();

        // Indexes
        builder.HasIndex(p => p.TrackingCode).IsUnique();
        builder.HasIndex(p => new { p.Status, p.CurrentHubId });
        builder.HasIndex(p => p.ReceiverPhone);

        // Enum conversions
        builder.Property(p => p.Status).HasConversion<string>();

        // Enable 2-way navigation for the tracking logs
        builder.HasMany(p => p.TrackingLogs).WithOne().HasForeignKey(tl => tl.ParcelId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.BagItems).WithOne().HasForeignKey(bi => bi.ParcelId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(p => p.ShipperAssignments).WithOne().HasForeignKey(sa => sa.ParcelId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(p => p.CodRecords).WithOne().HasForeignKey(cr => cr.ParcelId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(p => p.ParcelLocations).WithOne().HasForeignKey(pl => pl.ParcelId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(p => p.ReturnOrder).WithOne().HasForeignKey<ReturnOrder>(ro => ro.ParcelId).OnDelete(DeleteBehavior.Restrict);
    }
}

// ═══════════════════════════════════════════
// 🏗️ HUB & WAREHOUSE
// ═══════════════════════════════════════════
public class HubConfiguration : IEntityTypeConfiguration<Hub>
{
    public void Configure(EntityTypeBuilder<Hub> builder)
    {
        builder.ToTable("hubs");
        builder.HasIndex(h => h.HubCode).IsUnique();
        builder.Property(h => h.RowVersion).IsRowVersion();
    }
}

public class BagConfiguration : IEntityTypeConfiguration<Bag>
{
    public void Configure(EntityTypeBuilder<Bag> builder)
    {
        builder.ToTable("bags");
        builder.HasOne<Hub>().WithMany().HasForeignKey(b => b.FromHubId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Hub>().WithMany().HasForeignKey(b => b.ToHubId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(b => b.Packages).WithOne().HasForeignKey(bi => bi.BagId).OnDelete(DeleteBehavior.Cascade);

        builder.Property(b => b.Status).HasConversion<string>();
        builder.Property(b => b.RowVersion).IsRowVersion();
    }
}

public class BagItemConfiguration : IEntityTypeConfiguration<BagItem>
{
    public void Configure(EntityTypeBuilder<BagItem> builder)
    {
        builder.ToTable("bag_items");
        builder.HasIndex(bi => new { bi.BagId, bi.ParcelId });
    }
}

public class RackConfiguration : IEntityTypeConfiguration<Rack>
{
    public void Configure(EntityTypeBuilder<Rack> builder)
    {
        builder.ToTable("racks");
        builder.HasOne<Hub>().WithMany().HasForeignKey(r => r.HubId).OnDelete(DeleteBehavior.Cascade);
        builder.Ignore(r => r.IsFull);
    }
}

public class ParcelLocationConfiguration : IEntityTypeConfiguration<ParcelLocation>
{
    public void Configure(EntityTypeBuilder<ParcelLocation> builder)
    {
        builder.ToTable("parcel_locations");
        builder.HasOne<Rack>().WithMany().HasForeignKey(pl => pl.RackId).OnDelete(DeleteBehavior.Restrict);
    }
}

// ═══════════════════════════════════════════
// 👤 USERS & MERCHANTS
// ═══════════════════════════════════════════
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasOne<Hub>().WithMany().HasForeignKey(u => u.HubId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(u => u.EmployeeCode).IsUnique();

        builder.Property(u => u.Role).HasConversion<string>();
        builder.Property(u => u.RowVersion).IsRowVersion();
    }
}

public class MerchantConfiguration : IEntityTypeConfiguration<Merchant>
{
    public void Configure(EntityTypeBuilder<Merchant> builder)
    {
        builder.ToTable("merchants");
        builder.HasIndex(m => m.MerchantCode).IsUnique();
    }
}

// ═══════════════════════════════════════════
// 📜 LOGS & ASSIGNMENTS
// ═══════════════════════════════════════════
public class TrackingLogConfiguration : IEntityTypeConfiguration<TrackingLog>
{
    public void Configure(EntityTypeBuilder<TrackingLog> builder)
    {
        builder.ToTable("tracking_logs");
        builder.HasOne<Hub>().WithMany().HasForeignKey(tl => tl.HubId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(tl => new { tl.ParcelId, tl.CreatedTime });
        
        builder.Property(tl => tl.FromStatus).HasConversion<string>();
        builder.Property(tl => tl.ToStatus).HasConversion<string>();
    }
}

public class ShipperAssignmentConfiguration : IEntityTypeConfiguration<ShipperAssignment>
{
    public void Configure(EntityTypeBuilder<ShipperAssignment> builder)
    {
        builder.ToTable("shipper_assignments");
        builder.HasOne<User>().WithMany().HasForeignKey(sa => sa.ShipperId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Hub>().WithMany().HasForeignKey(sa => sa.HubId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(sa => new { sa.ShipperId, sa.Status });
    }
}

// ═══════════════════════════════════════════
// 💰 FINANCE & CONFIG
// ═══════════════════════════════════════════
public class CodRecordConfiguration : IEntityTypeConfiguration<CodRecord>
{
    public void Configure(EntityTypeBuilder<CodRecord> builder)
    {
        builder.ToTable("cod_records");
        builder.Property(cr => cr.Amount).HasColumnType("decimal(18,2)");
    }
}

public class CodSettlementConfiguration : IEntityTypeConfiguration<CodSettlement>
{
    public void Configure(EntityTypeBuilder<CodSettlement> builder)
    {
        builder.ToTable("cod_settlements");
        builder.HasOne<Merchant>().WithMany().HasForeignKey(cs => cs.MerchantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(cs => cs.ShipperId).OnDelete(DeleteBehavior.Restrict);
        builder.Property(cs => cs.TotalCollected).HasColumnType("decimal(18,2)");
        builder.Property(cs => cs.TotalSubmitted).HasColumnType("decimal(18,2)");
        builder.Property(cs => cs.TotalSettled).HasColumnType("decimal(18,2)");
        builder.Property(cs => cs.RowVersion).IsRowVersion();
    }
}

public class CodSettlementItemConfiguration : IEntityTypeConfiguration<CodSettlementItem>
{
    public void Configure(EntityTypeBuilder<CodSettlementItem> builder)
    {
        builder.ToTable("cod_settlement_items");
        builder.HasOne<CodSettlement>().WithMany().HasForeignKey(csi => csi.SettlementId).OnDelete(DeleteBehavior.Cascade);
        builder.Property(csi => csi.Amount).HasColumnType("decimal(18,2)");
    }
}

public class PickupRequestConfiguration : IEntityTypeConfiguration<PickupRequest>
{
    public void Configure(EntityTypeBuilder<PickupRequest> builder)
    {
        builder.ToTable("pickup_requests");
        builder.HasOne<Merchant>().WithMany().HasForeignKey(pr => pr.MerchantId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class ReturnOrderConfiguration : IEntityTypeConfiguration<ReturnOrder>
{
    public void Configure(EntityTypeBuilder<ReturnOrder> builder)
    {
        builder.ToTable("return_orders");
        builder.HasOne<Hub>().WithMany().HasForeignKey(ro => ro.ReturnHubId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class FeeConfigConfiguration : IEntityTypeConfiguration<FeeConfig>
{
    public void Configure(EntityTypeBuilder<FeeConfig> builder)
    {
        builder.ToTable("fee_configs");
        
        // Nối dây Foreign Key cho ServiceType và Zone
        builder.HasOne<ServiceType>().WithMany().HasForeignKey(f => f.ServiceTypeId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Zone>().WithMany().HasForeignKey(f => f.ZoneId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(f => new { f.ZoneId, f.ServiceTypeId });

        builder.OwnsOne(f => f.BasePrice, pb => {
            pb.Property(m => m.Amount).HasColumnName("base_price_amount").HasColumnType("decimal(18,2)");
            pb.Property(m => m.Currency).HasColumnName("base_price_currency").HasMaxLength(3);
        });
        builder.OwnsOne(f => f.ExtraPricePerKg, pb => {
            pb.Property(m => m.Amount).HasColumnName("extra_price_amount").HasColumnType("decimal(18,2)");
            pb.Property(m => m.Currency).HasColumnName("extra_price_currency").HasMaxLength(3);
        });
    }
}

public class ServiceTypeConfiguration : IEntityTypeConfiguration<ServiceType>
{
    public void Configure(EntityTypeBuilder<ServiceType> builder)
    {
        builder.ToTable("service_types");
        builder.HasIndex(s => s.Code).IsUnique();

        builder.HasData(
            new ServiceType 
            { 
                Id = Guid.Parse("aa1e9c52-7360-4927-aa72-5b91cf8e9661"),
                Code = "STANDARD", 
                Name = "Giao hàng Tiêu chuẩn", 
                BaseFee = 15000, 
                EstimatedDays = "3-5 ngày" 
            },
            new ServiceType 
            { 
                Id = Guid.Parse("bb1e9c52-7360-4927-aa72-5b91cf8e9662"),
                Code = "FAST", 
                Name = "Giao hàng Nhanh", 
                BaseFee = 25000, 
                EstimatedDays = "1-2 ngày" 
            },
            new ServiceType 
            { 
                Id = Guid.Parse("cc1e9c52-7360-4927-aa72-5b91cf8e9663"),
                Code = "EXPRESS", 
                Name = "Giao hàng Hỏa tốc", 
                BaseFee = 45000, 
                EstimatedDays = "Trong ngày" 
            }
        );
    }
}

public class ZoneConfiguration : IEntityTypeConfiguration<Zone>
{
    public void Configure(EntityTypeBuilder<Zone> builder)
    {
        builder.ToTable("zones");
        builder.HasIndex(z => z.Code).IsUnique();
    }
}

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");
        builder.Property(a => a.Changes).HasColumnType("nvarchar(max)");
    }
}
