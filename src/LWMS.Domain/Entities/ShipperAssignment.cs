using LWMS.Domain.Common;

namespace LWMS.Domain.Entities;

/// <summary>
/// 📋 SHIPPER ASSIGNMENT — Phân công shipper lấy/giao hàng.
/// Lifecycle: ASSIGNED → IN_PROGRESS → DONE / FAILED.
/// </summary>
public class ShipperAssignment : BaseEntity
{
    /// <summary>Bưu kiện cần xử lý</summary>
    public Guid ParcelId { get; set; }

    /// <summary>Shipper (User) được phân công</summary>
    public Guid ShipperId { get; set; }

    /// <summary>Hub xuất phát</summary>
    public Guid HubId { get; set; }

    /// <summary>Loại phân công: DELIVERY (giao) / PICKUP (lấy)</summary>
    public string Type { get; set; } = "DELIVERY";

    /// <summary>Trạng thái: ASSIGNED / IN_PROGRESS / DONE / FAILED</summary>
    public string Status { get; set; } = "ASSIGNED";

    /// <summary>Thời điểm phân công</summary>
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Thời điểm shipper bắt đầu xử lý</summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>Thời điểm hoàn thành</summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>Lý do thất bại (nếu Status = FAILED)</summary>
    public string? FailedReason { get; set; }

    public Parcel Parcel { get; set; } = null!;
    public User Shipper { get; set; } = null!;
    public Hub Hub { get; set; } = null!;
}