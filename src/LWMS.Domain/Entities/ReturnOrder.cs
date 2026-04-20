using LWMS.Domain.Common;
using LWMS.Domain.Enums;

namespace LWMS.Domain.Entities;

/// <summary>
/// Thực thể quản lý đơn hàng hoàn trả (Return Order).
/// Xảy ra khi Parcel không thể giao thành công hoặc Merchant yêu cầu trả hàng.
/// </summary>
public class ReturnOrder : BaseEntity
{
    /// <summary>
    /// Tham chiếu đến bưu kiện gốc.
    /// </summary>
    public Guid ParcelId { get; set; }

    /// <summary>
    /// Mã vận đơn của đơn hoàn (thường bắt đầu bằng chữ R hoặc mã gốc + suffix).
    /// </summary>
    public string ReturnTrackingCode { get; set; } = string.Empty;

    /// <summary>
    /// Lý do hoàn trả (Lấy từ Enum ReturnReason).
    /// </summary>
    public ReturnReason Reason { get; set; }

    /// <summary>
    /// Ghi chú chi tiết về tình trạng hàng khi hoàn.
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// ID nhân viên/shipper yêu cầu tạo đơn hoàn.
    /// </summary>
    public Guid RequestedById { get; set; }

    /// <summary>
    /// Trạng thái của đơn hoàn (Pending, InTransit, ReturnedToMerchant).
    /// </summary>
    public string Status { get; set; } = "PENDING";
}