using LWMS.Domain.Common;

namespace LWMS.Domain.Entities;

/// <summary>
/// 📦 BAG ITEM — Liên kết Bag ↔ Parcel (bảng trung gian).
/// Ghi nhận ai đã đưa parcel vào bag và khi nào.
/// </summary>
public class BagItem : BaseEntity
{
    /// <summary>Bưu kiện nào</summary>
    public Guid ParcelId { get; set; }

    /// <summary>Thuộc bao nào</summary>
    public Guid BagId { get; set; }

    /// <summary>Thời điểm đưa parcel vào bag</summary>
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Nhân viên kho thực hiện (User Id)</summary>
    public Guid AddedBy { get; set; }

    public Bag Bag { get; set; } = null!;
    public Parcel Parcel { get; set; } = null!;
}