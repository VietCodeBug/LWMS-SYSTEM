using LWMS.Domain.Entities;
using LWMS.Domain.Enums;

namespace LWMS.Infrastructure.Data;

/// <summary>
/// 🧪 NƠI QUẢN LÝ DỮ LIỆU MẪU (TEST DATA) TOÀN HỆ THỐNG
/// Hướng dẫn: 
/// - Mọi dữ liệu dùng để test, demo sẽ được khai báo tại đây.
/// - Khi cần lên Production, chỉ cần xóa nội dung trong các phương thức này hoặc thay bằng dữ liệu thật.
/// - Không nên sửa trực tiếp trong các file *Configuration.cs để tránh nhầm lẫn.
/// </summary>
public static class InitialSeedData
{
    // ═══════════════════════════════════════════
    // 1. DỮ LIỆU LOẠI DỊCH VỤ (Service Types)
    // ═══════════════════════════════════════════
    public static IEnumerable<ServiceType> GetServiceTypes()
    {
        return new List<ServiceType>
        {
            new() { 
                Id = Guid.Parse("aa1e9c52-7360-4927-aa72-5b91cf8e9661"),
                Code = "STANDARD", 
                Name = "Giao hàng Tiêu chuẩn", 
                BaseFee = 15000, 
                EstimatedDays = "3-5 ngày" 
            },
            new() { 
                Id = Guid.Parse("bb1e9c52-7360-4927-aa72-5b91cf8e9662"),
                Code = "FAST", 
                Name = "Giao hàng Nhanh", 
                BaseFee = 25000, 
                EstimatedDays = "1-2 ngày" 
            },
            new() { 
                Id = Guid.Parse("cc1e9c52-7360-4927-aa72-5b91cf8e9663"),
                Code = "EXPRESS", 
                Name = "Giao hàng Hỏa tốc", 
                BaseFee = 45000, 
                EstimatedDays = "Trong ngày" 
            }
        };
    }

    // ═══════════════════════════════════════════
    // 2. DỮ LIỆU HUB (Kho bãi mẫu)
    // ═══════════════════════════════════════════
    public static IEnumerable<Hub> GetHubs()
    {
        return new List<Hub>
        {
            new() { 
                Id = Guid.Parse("001e9c52-7360-4927-aa72-5b91cf8e9661"),
                HubCode = "HN-01", 
                Name = "Hub Hà Nội - Trung tâm", 
                Address = "Số 1 Cầu Giấy, Hà Nội", 
                ProvinceCode = "HN",
                HubType = "SORTING_CENTER",
                HubLevel = 1,
                Capacity = 10000
            },
            new() { 
                Id = Guid.Parse("001e9c52-7360-4927-aa72-5b91cf8e9662"),
                HubCode = "HCM-01", 
                Name = "Hub TP.HCM - Miền Nam", 
                Address = "Số 100 Quận 1, TP.HCM", 
                ProvinceCode = "HCM",
                HubType = "SORTING_CENTER",
                HubLevel = 1,
                Capacity = 12000
            }
        };
    }

    // ═══════════════════════════════════════════
    // 3. DỮ LIỆU MERCHANT (Khách hàng mẫu)
    // ═══════════════════════════════════════════
    public static IEnumerable<Merchant> GetMerchants()
    {
        return new List<Merchant>
        {
            new() { 
                Id = Guid.Parse("221e9c52-7360-4927-aa72-5b91cf8e9661"),
                MerchantCode = "DEMO-MERC",
                Name = "Giao Hàng Demo Store",
                Phone = "0888888888",
                Email = "contact@demo-merc.com",
                ApiKey = "lwms_test_api_key_2026",
                IsActive = true
            }
        };
    }

    // ═══════════════════════════════════════════
    // 4. DỮ LIỆU USER (Tài khoản Test)
    // ═══════════════════════════════════════════
    public static IEnumerable<User> GetUsers()
    {
        return new List<User>
        {
            new() { 
                Id = Guid.Parse("111e9c52-7360-4927-aa72-5b91cf8e9661"),
                EmployeeCode = "ADM-01",
                FullName = "Hệ thống Admin",
                Phone = "0999999999",
                // Pass: Admin@123
                PasswordHash = "$2a$11$mC7p989S.n6Y7R3YjX2YPO.Y8Y7R3YjX2YPO.Y8Y7R3YjX2YPO.",
                Role = UserRole.Admin,
                IsActive = true
            }
        };
    }
}
