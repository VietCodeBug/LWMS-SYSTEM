using LWMS.Domain.Common;
using LWMS.Domain.Enums;

namespace LWMS.Domain.Entities;

/// <summary>
/// 📜 TRACKING LOG — Lịch sử trạng thái bưu kiện (Append-only).
/// Mỗi lần bưu kiện được scan, chuyển trạng thái, hoặc gặp sự cố → ghi 1 dòng.
/// </summary>
public class TrackingLog : BaseEntity
{
    /// <summary>Bưu kiện được tracking</summary>
    public Guid ParcelId { get; set; }

    /// <summary>Trạng thái trước</summary>
    public ParcelStatus FromStatus { get; set; }

    /// <summary>Trạng thái sau</summary>
    public ParcelStatus ToStatus { get; set; }

    /// <summary>Người thực hiện (User Id)</summary>
    public Guid ActorId { get; set; }

    /// <summary>Tại Hub nào (null = ngoài Hub, VD: shipper đang giao)</summary>
    public Guid? HubId { get; set; }

    /// <summary>Vai trò người thực hiện: STAFF / SHIPPER / SYSTEM / ADMIN</summary>
    public string ActorRole { get; set; } = "STAFF";

    /// <summary>Loại sự kiện: SCAN / STATUS_CHANGE / FAIL / TRANSIT / RETURN</summary>
    public string EventType { get; set; } = "STATUS_CHANGE";

    /// <summary>Địa điểm (text mô tả, VD: "Hub Hà Nội - Kho A")</summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>Lý do (nếu fail): RECIPIENT_ABSENT, WRONG_ADDRESS, REFUSED...</summary>
    public string? ReasonCode { get; set; }

    /// <summary>URL ảnh chụp bằng chứng (nếu có)</summary>
    public string? PhotoUrl { get; set; }

    /// <summary>Ghi chú bổ sung</summary>
    public string? Note { get; set; }

    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
}