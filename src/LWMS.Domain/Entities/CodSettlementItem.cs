using LWMS.Domain.Common;

namespace LWMS.Domain.Entities;

/// <summary>
/// 💳 COD SETTLEMENT ITEM — Chi tiết từng đơn hàng trong 1 phiên quyết toán.
/// Thay thế cho List&lt;Guid&gt; ParcelIds (design sai).
/// Settlement 1-N SettlementItem N-1 Parcel.
/// </summary>
public class CodSettlementItem : BaseEntity
{
    /// <summary>Thuộc phiên quyết toán nào</summary>
    public Guid SettlementId { get; set; }

    /// <summary>Bưu kiện nào</summary>
    public Guid ParcelId { get; set; }

    /// <summary>Số tiền COD của parcel này</summary>
    public decimal Amount { get; set; }
}
