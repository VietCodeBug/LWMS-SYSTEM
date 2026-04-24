using LWMS.Domain.Common;

namespace LWMS.Domain.Entities;

/// <summary>
/// 💳 COD SETTLEMENT — Quyết toán tiền COD giữa Shipper → Hub → Merchant.
/// Mỗi Settlement chứa nhiều CodSettlementItem (từng đơn hàng).
/// </summary>
public class CodSettlement : BaseEntity, IMustHaveMerchant
{
    /// <summary>Merchant nhận tiền quyết toán</summary>
    public Guid MerchantId { get; set; }

    /// <summary>Shipper đã thu tiền từ khách</summary>
    public Guid ShipperId { get; set; }

    /// <summary>Tổng tiền shipper thu từ khách</summary>
    public decimal TotalCollected { get; set; }

    /// <summary>Tiền shipper đã nộp về Hub</summary>
    public decimal TotalSubmitted { get; set; }

    /// <summary>Tiền đã chuyển cho merchant</summary>
    public decimal TotalSettled { get; set; }

    /// <summary>Trạng thái: PENDING / COMPLETED / FAILED</summary>
    public string Status { get; set; } = "PENDING";

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? SettledDate { get; set; }

    public Merchant Merchant { get; set; } = null!;
    public User Shipper { get; set; } = null!;
    public ICollection<CodSettlementItem> Items { get; set; } = new List<CodSettlementItem>();
}