using LWMS.Domain.Common;

namespace LWMS.Domain.Entities;

/// <summary>
/// Thực thể Vị trí bưu kiện (Parcel Location).
/// Dùng để lưu vết xem một bưu kiện đang nằm chính xác ở đâu trong kho.
/// </summary>
public class ParcelLocation : BaseEntity
{
    /// <summary>
    /// Bưu kiện được định vị.
    /// </summary>
    public Guid ParcelId { get; set; }

    /// <summary>
    /// Kệ hàng hiện tại.
    /// </summary>
    public Guid RackId { get; set; }

    /// <summary>
    /// Thời điểm đưa bưu kiện vào kệ.
    /// </summary>
    public DateTime InDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// ID nhân viên kho thực hiện việc quét (scan) đưa hàng vào kệ.
    /// </summary>
    public Guid ActorId { get; set; }

    /// <summary>
    /// Ghi chú thêm (VD: Vị trí tầng 2 của kệ).
    /// </summary>
    public string Note { get; set; } = string.Empty;
}