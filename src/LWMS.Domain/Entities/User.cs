
using LWMS.Domain.Common;
using LWMS.Domain.Enums;

namespace LWMS.Domain.Entities;

/// <summary>
/// 🟧 USER — Nhân viên hệ thống (Admin, Staff, Shipper).
/// Shipper có thêm thông tin xe và capacity.
/// </summary>
public class User : BaseEntity
{
    /// <summary>Mã nhân viên duy nhất</summary>
    public string EmployeeCode { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Vai trò: Admin, Manager, Staff, Shipper</summary>
    public UserRole Role { get; set; }

    /// <summary>Hub mà nhân viên thuộc về</summary>
    public Guid? HubId { get; set; }

    /// <summary>Merchant mà nhân viên thuộc về (nếu có)</summary>
    public Guid? MerchantId { get; set; }

    /// <summary>Tài khoản có đang hoạt động không</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Lần đăng nhập gần nhất</summary>
    public DateTime? LastLogin { get; set; }

    // ── Shipper specific fields ──

    /// <summary>Loại phương tiện: MOTORBIKE / VAN / TRUCK (null nếu không phải shipper)</summary>
    public string? VehicleType { get; set; }

    /// <summary>Số đơn tối đa shipper xử lý được trong ngày</summary>
    public int? ShipperCapacity { get; set; }
}