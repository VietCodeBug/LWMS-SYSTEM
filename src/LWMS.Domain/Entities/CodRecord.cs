using LWMS.Domain.Common;

namespace LWMS.Domain.Entities;

/// <summary>
/// 💰 COD RECORD — Ghi nhận tiền thu hộ cho từng Parcel.
/// Mỗi parcel có COD sẽ có 1 CodRecord theo dõi trạng thái thu tiền.
/// </summary>
public class CodRecord : BaseEntity
{
    /// <summary>Bưu kiện có COD</summary>
    public Guid ParcelId { get; set; }
    public Parcel Parcel { get; set; } = null!;

    /// <summary>Số tiền COD cần thu</summary>
    public decimal Amount { get; set; }

    /// <summary>Đã thu tiền từ khách chưa</summary>
    public bool IsCollected { get; set; }

    /// <summary>Trạng thái: PENDING / COLLECTED / FAILED</summary>
    public string Status { get; set; } = "PENDING";

    /// <summary>Thời điểm thu tiền thành công</summary>
    public DateTime? CollectedAt { get; set; }

    /// <summary>User Id của shipper đã thu tiền</summary>
    public Guid? CollectedBy { get; set; }

    public DateTime? SubmittedAt { get; set; }
    public DateTime? SettledAt { get; set; }
}