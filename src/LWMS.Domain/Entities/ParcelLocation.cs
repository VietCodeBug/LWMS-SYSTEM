using LWMS.Domain.Common;

namespace LWMS.Domain.Entities;

/// <summary>
/// 📍 PARCEL LOCATION — Theo dõi vị trí bưu kiện trên kệ trong kho.
/// Ghi nhận cả IN (đưa vào) và OUT (lấy ra).
/// </summary>
public class ParcelLocation : BaseEntity
{
    /// <summary>Bưu kiện được định vị</summary>
    public Guid ParcelId { get; set; }

    /// <summary>Kệ hàng hiện tại</summary>
    public Guid RackId { get; set; }

    /// <summary>Nhân viên kho scan đưa hàng vào/ra kệ</summary>
    public Guid ActorId { get; set; }

    /// <summary>Loại di chuyển: IN (đưa vào kệ) / OUT (lấy ra khỏi kệ)</summary>
    public string MovementType { get; set; } = "IN";

    /// <summary>Thời điểm đưa bưu kiện vào kệ</summary>
    public DateTime InDate { get; set; } = DateTime.UtcNow;

    /// <summary>Thời điểm lấy bưu kiện ra khỏi kệ (null = đang trên kệ)</summary>
    public DateTime? OutDate { get; set; }

    /// <summary>Ghi chú (VD: Tầng 2 kệ, vị trí góc trái)</summary>
    public string Note { get; set; } = string.Empty;
}