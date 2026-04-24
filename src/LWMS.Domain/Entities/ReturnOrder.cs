using LWMS.Domain.Common;
using LWMS.Domain.Enums;

namespace LWMS.Domain.Entities;

/// <summary>
/// 🔄 RETURN ORDER — Đơn hoàn trả khi giao thất bại hoặc Merchant yêu cầu.
/// Lifecycle: PENDING → IN_TRANSIT → RETURNED_TO_MERCHANT / DESTROYED.
/// </summary>
public class ReturnOrder : BaseEntity
{
    /// <summary>Tham chiếu đến bưu kiện gốc</summary>
    public Guid ParcelId { get; set; }

    /// <summary>Mã vận đơn đơn hoàn (prefix R + mã gốc)</summary>
    public string ReturnTrackingCode { get; set; } = string.Empty;

    /// <summary>Lý do hoàn trả</summary>
    public ReturnReason Reason { get; set; }

    /// <summary>Ghi chú chi tiết</summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>Người yêu cầu tạo đơn hoàn (Shipper/Staff)</summary>
    public Guid RequestedById { get; set; }

    /// <summary>Người duyệt đơn hoàn (Manager/Admin)</summary>
    public Guid? ApprovedBy { get; set; }

    /// <summary>Hub trả hàng về (thường = OriginHub của Parcel)</summary>
    public Guid? ReturnHubId { get; set; }

    /// <summary>Kiểu hoàn: TO_SENDER (trả merchant) / DESTROY (hủy)</summary>
    public string ReturnType { get; set; } = "TO_SENDER";

    /// <summary>Trạng thái: PENDING / IN_TRANSIT / RETURNED / DESTROYED</summary>
    public string Status { get; set; } = "PENDING";

    public Parcel Parcel { get; set; } = null!;
    public Hub? ReturnHub { get; set; }
}