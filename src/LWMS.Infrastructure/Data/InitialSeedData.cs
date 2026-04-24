using LWMS.Domain.Entities;
using LWMS.Domain.Enums;
using LWMS.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using LWMS.Application.Common.Interfaces;

namespace LWMS.Infrastructure.Data;

public static class InitialSeedData
{
    public static IEnumerable<ServiceType> GetServiceTypes()
    {
        return new List<ServiceType>
        {
            new() { Id = Guid.Parse("aa1e9c52-7360-4927-aa72-5b91cf8e9661"), Code = "STANDARD", Name = "Giao hàng Tiêu chuẩn", BaseFee = 15000, EstimatedDays = "3-5 ngày" },
            new() { Id = Guid.Parse("bb1e9c52-7360-4927-aa72-5b91cf8e9662"), Code = "FAST", Name = "Giao hàng Nhanh", BaseFee = 25000, EstimatedDays = "1-2 ngày" },
            new() { Id = Guid.Parse("cc1e9c52-7360-4927-aa72-5b91cf8e9663"), Code = "EXPRESS", Name = "Giao hàng Hỏa tốc", BaseFee = 45000, EstimatedDays = "Trong ngày" }
        };
    }

    public static IEnumerable<Hub> GetHubs()
    {
        return new List<Hub>
        {
            new() {
                Id = Guid.Parse("001e9c52-7360-4927-aa72-5b91cf8e9661"), HubCode = "HN-CORE", Name = "Tổng kho Hà Nội", Address = "Giải Phóng, Hà Nội",
                ProvinceCode = "HN", HubType = "SORTING_CENTER", HubLevel = 1, Capacity = 50000
            },
            new() {
                Id = Guid.Parse("001e9c52-7360-4927-aa72-5b91cf8e9662"), HubCode = "HCM-CORE", Name = "Tổng kho TP.HCM", Address = "Quận 12, TP.HCM",
                ProvinceCode = "HCM", HubType = "SORTING_CENTER", HubLevel = 1, Capacity = 60000
            }
        };
    }

    public static IEnumerable<Merchant> GetMerchants()
    {
        return new List<Merchant>
        {
            new() {
                Id = Guid.Parse("221e9c52-7360-4927-aa72-5b91cf8e9661"), MerchantCode = "SHOPEE-V", Name = "Shopee Vietnam", Phone = "19001221",
                Email = "contact@shopee.vn", ApiKey = "shopee_key_2026", IsActive = true
            },
            new() {
                Id = Guid.Parse("221e9c52-7360-4927-aa72-5b91cf8e9662"), MerchantCode = "TIKI-V", Name = "Tiki Trading", Phone = "19006035",
                Email = "hotro@tiki.vn", ApiKey = "tiki_key_2026", IsActive = true
            }
        };
    }

    public static async Task SeedAsync(AppDbContext context, IPasswordService passwordService)
    {
        var passHash = passwordService.HashPassword("Admin@123");
        var hnHubId = Guid.Parse("001e9c52-7360-4927-aa72-5b91cf8e9661");
        var hcmHubId = Guid.Parse("001e9c52-7360-4927-aa72-5b91cf8e9662");

        if (!await context.ServiceTypes.AnyAsync()) { await context.ServiceTypes.AddRangeAsync(GetServiceTypes()); }
        if (!await context.Hubs.AnyAsync()) { await context.Hubs.AddRangeAsync(GetHubs()); }

        var merchants = GetMerchants();
        foreach (var merchant in merchants)
        {
            if (!await context.Merchants.AnyAsync(m => m.MerchantCode == merchant.MerchantCode))
            {
                await context.Merchants.AddAsync(merchant);
            }
        }
        await context.SaveChangesAsync();

        var users = new List<User>
        {
            new() { Id = Guid.Parse("111e9c52-7360-4927-aa72-5b91cf8e9661"), EmployeeCode = "ADMIN", FullName = "Hệ thống Admin", Phone = "0900000000", PasswordHash = passHash, Role = UserRole.Admin, IsActive = true },
            new() { Id = Guid.NewGuid(), EmployeeCode = "STAFF-HN", FullName = "Nhân viên Hà Nội", Phone = "0911111111", PasswordHash = passHash, Role = UserRole.Staff, HubId = hnHubId, IsActive = true },
            new() { Id = Guid.NewGuid(), EmployeeCode = "SHIPPER-HCM", FullName = "Shipper Sài Gòn", Phone = "0922222222", PasswordHash = passHash, Role = UserRole.Shipper, HubId = hcmHubId, IsActive = true, VehicleType = "MOTORBIKE", ShipperCapacity = 50 },
            new() { Id = Guid.NewGuid(), EmployeeCode = "MANAGER-HCM", FullName = "Quản lý HCM", Phone = "0933333333", PasswordHash = passHash, Role = UserRole.HubManager, HubId = hcmHubId, IsActive = true }
        };

        foreach (var user in users)
        {
            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.EmployeeCode == user.EmployeeCode);
            if (existingUser == null)
            {
                await context.Users.AddAsync(user);
            }
            else
            {
                existingUser.PasswordHash = user.PasswordHash;
                existingUser.FullName = user.FullName;
                existingUser.Role = user.Role;
                existingUser.HubId = user.HubId;
            }
        }

        await context.SaveChangesAsync();

        // Racks
        if (!await context.Racks.AnyAsync())
        {
            var hubs = await context.Hubs.ToListAsync();
            foreach (var hub in hubs)
            {
                var racks = Enumerable.Range(1, 3).Select(i => new Rack
                {
                    Id = Guid.NewGuid(),
                    HubId = hub.Id,
                    RackCode = $"{hub.HubCode}-KARE-{i}",
                    Capacity = 100
                });
                await context.Racks.AddRangeAsync(racks);
            }
            await context.SaveChangesAsync();
        }

        // 📦 SEED 10 REAL PARCELS
        var shopee = await context.Merchants.FirstAsync(m => m.MerchantCode == "SHOPEE-V");
        var tiki = await context.Merchants.FirstAsync(m => m.MerchantCode == "TIKI-V");
        var fastServiceId = Guid.Parse("bb1e9c52-7360-4927-aa72-5b91cf8e9662");

        var parcels = new List<Parcel>
        {
            new() {
                Id = Guid.NewGuid(), TrackingCode = "LWMS-TEST-001", MerchantId = shopee.Id, Status = ParcelStatus.Created,
                SenderName = "Kho Shopee Bắc Ninh", SenderPhone = "0988111222", SenderAddress = new Address("KCN Yên Phong", "Yên Phong", "Bắc Ninh", "VN"),
                ReceiverName = "Nguyễn Văn Tuân", ReceiverPhone = "0912345678", ReceiverAddress = new Address("123 Giải Phóng", "Hai Bà Trưng", "Hà Nội", "VN"),
                Weight = 1.5m, CodAmount = new Money(500000, "VND"), Notes = "[TEST DATA]", CreatedAt = DateTime.UtcNow, ServiceTypeId = fastServiceId,
                OriginHubId = hnHubId, DestHubId = hnHubId
            },
            new() {
                Id = Guid.NewGuid(), TrackingCode = "LWMS-TEST-002", MerchantId = shopee.Id, Status = ParcelStatus.Picked,
                SenderName = "Shopee Mall", SenderPhone = "0988111222", SenderAddress = new Address("72 Lê Thánh Tôn", "Quận 1", "TP.HCM", "VN"),
                ReceiverName = "Trần Thị Lan", ReceiverPhone = "0944555666", ReceiverAddress = new Address("456 Lê Lợi", "Hải Châu", "Đà Nẵng", "VN"),
                Weight = 0.5m, CodAmount = new Money(200000, "VND"), Notes = "[TEST DATA]", CreatedAt = DateTime.UtcNow, ServiceTypeId = fastServiceId,
                OriginHubId = hcmHubId, DestHubId = hcmHubId
            },
            new() {
                Id = Guid.NewGuid(), TrackingCode = "LWMS-TEST-003", MerchantId = tiki.Id, Status = ParcelStatus.ArrivedHub,
                SenderName = "Tiki Trading Nhà Bè", SenderPhone = "0977333444", SenderAddress = new Address("Kho Nhà Bè", "Nhà Bè", "TP.HCM", "VN"),
                ReceiverName = "Lê Hồng Phong", ReceiverPhone = "0966777888", ReceiverAddress = new Address("789 Phan Chu Trinh", "TP Huế", "Thừa Thiên Huế", "VN"),
                Weight = 2.0m, CodAmount = new Money(1200000, "VND"), Notes = "[TEST DATA]", CreatedAt = DateTime.UtcNow.AddHours(-5), ServiceTypeId = fastServiceId,
                CurrentHubId = hcmHubId, OriginHubId = hcmHubId, DestHubId = hcmHubId
            },
            new() {
                Id = Guid.NewGuid(), TrackingCode = "LWMS-TEST-004", MerchantId = shopee.Id, Status = ParcelStatus.OutForDelivery,
                SenderName = "Shop Mỹ Phẩm ABC", SenderPhone = "0955999000", SenderAddress = new Address("Cầu Giấy", "Cầu Giấy", "Hà Nội", "VN"),
                ReceiverName = "Hoàng Anh Đức", ReceiverPhone = "0922111333", ReceiverAddress = new Address("321 Trần Hưng Đạo", "TP Nam Định", "Nam Định", "VN"),
                Weight = 0.2m, CodAmount = new Money(0, "VND"), Notes = "[TEST DATA]", CreatedAt = DateTime.UtcNow.AddDays(-1), ServiceTypeId = fastServiceId,
                CurrentHubId = hnHubId, OriginHubId = hnHubId, DestHubId = hnHubId
            },
            new() {
                Id = Guid.NewGuid(), TrackingCode = "LWMS-TEST-005", MerchantId = tiki.Id, Status = ParcelStatus.Delivered,
                SenderName = "Tiki Now", SenderPhone = "0933222111", SenderAddress = new Address("Tân Bình", "Tân Bình", "TP.HCM", "VN"),
                ReceiverName = "Phạm Minh Tài", ReceiverPhone = "0911222333", ReceiverAddress = new Address("55 Hai Bà Trưng", "Ninh Kiều", "Cần Thơ", "VN"),
                Weight = 5.0m, CodAmount = new Money(3500000, "VND"), Notes = "[TEST DATA]", CreatedAt = DateTime.UtcNow.AddDays(-2), ServiceTypeId = fastServiceId,
                OriginHubId = hcmHubId, DestHubId = hcmHubId
            },
            new() {
                Id = Guid.NewGuid(), TrackingCode = "LWMS-TEST-006", MerchantId = shopee.Id, Status = ParcelStatus.FailedDelivery,
                SenderName = "Thế Giới Di Động", SenderPhone = "18001060", SenderAddress = new Address("TP. Thủ Đức", "Thủ Đức", "TP.HCM", "VN"),
                ReceiverName = "Vũ Mạnh Hùng", ReceiverPhone = "0977888999", ReceiverAddress = new Address("88 Lý Thái Tổ", "Pleiku", "Gia Lai", "VN"),
                Weight = 0.8m, CodAmount = new Money(15000000, "VND"), Notes = "[TEST DATA]", CreatedAt = DateTime.UtcNow.AddDays(-1), ServiceTypeId = fastServiceId,
                OriginHubId = hcmHubId, DestHubId = hcmHubId
            },
            new() {
                Id = Guid.NewGuid(), TrackingCode = "LWMS-TEST-007", MerchantId = shopee.Id, Status = ParcelStatus.ArrivedHub,
                SenderName = "Apple Store VN", SenderPhone = "0900111222", SenderAddress = new Address("Quận 3", "Quận 3", "TP.HCM", "VN"),
                ReceiverName = "Đặng Quang Tèo", ReceiverPhone = "0933444555", ReceiverAddress = new Address("99 Xuân Thủy", "Cầu Giấy", "Hà Nội", "VN"),
                Weight = 0.3m, CodAmount = new Money(0, "VND"), Notes = "[TEST DATA]", CreatedAt = DateTime.UtcNow.AddHours(-2), ServiceTypeId = fastServiceId,
                CurrentHubId = hcmHubId, OriginHubId = hcmHubId, DestHubId = hnHubId
            },
            new() {
                Id = Guid.NewGuid(), TrackingCode = "LWMS-TEST-008", MerchantId = tiki.Id, Status = ParcelStatus.Created,
                SenderName = "Samsung Brand Shop", SenderPhone = "08001234", SenderAddress = new Address("Quận 7", "Quận 7", "TP.HCM", "VN"),
                ReceiverName = "Bùi Tiến Dũng", ReceiverPhone = "0912111222", ReceiverAddress = new Address("10 Lạch Tray", "Ngô Quyền", "Hải Phòng", "VN"),
                Weight = 1.2m, CodAmount = new Money(8000000, "VND"), Notes = "[TEST DATA]", CreatedAt = DateTime.UtcNow, ServiceTypeId = fastServiceId,
                OriginHubId = hcmHubId, DestHubId = hcmHubId
            },
            new() {
                Id = Guid.NewGuid(), TrackingCode = "LWMS-TEST-009", MerchantId = shopee.Id, Status = ParcelStatus.Returning,
                SenderName = "Shop Quần Áo Z", SenderPhone = "0966444222", SenderAddress = new Address("Gò Vấp", "Gò Vấp", "TP.HCM", "VN"),
                ReceiverName = "Mai Phương Thúy", ReceiverPhone = "0944000111", ReceiverAddress = new Address("22 Nguyễn Chí Thanh", "Đống Đa", "Hà Nội", "VN"),
                Weight = 0.6m, CodAmount = new Money(450000, "VND"), Notes = "[TEST DATA]", CreatedAt = DateTime.UtcNow.AddDays(-3), ServiceTypeId = fastServiceId,
                OriginHubId = hcmHubId, DestHubId = hcmHubId
            },
            new() {
                Id = Guid.NewGuid(), TrackingCode = "LWMS-TEST-010", MerchantId = shopee.Id, Status = ParcelStatus.Returned,
                SenderName = "Phụ Kiện Điện Thoaị", SenderPhone = "0922000222", SenderAddress = new Address("Thanh Xuân", "Thanh Xuân", "Hà Nội", "VN"),
                ReceiverName = "Trịnh Công Sơn", ReceiverPhone = "0911000111", ReceiverAddress = new Address("11 Lê Lợi", "TP Vinh", "Nghệ An", "VN"),
                Weight = 0.1m, CodAmount = new Money(150000, "VND"), Notes = "[TEST DATA]", CreatedAt = DateTime.UtcNow.AddDays(-5), ServiceTypeId = fastServiceId,
                OriginHubId = hnHubId, DestHubId = hnHubId
            }
        };

        foreach (var parcel in parcels)
        {
            if (!await context.Parcels.AnyAsync(p => p.TrackingCode == parcel.TrackingCode))
            {
                await context.Parcels.AddAsync(parcel);
                Console.WriteLine($"[SEED] Added parcel: {parcel.TrackingCode}");
            }
        }

        await context.SaveChangesAsync();
    }
}
