using LWMS.Domain.Common;

namespace LWMS.Domain.Entities;

public class CodSettlement : BaseEntity
{
    public Guid MerchantId { get; set; }

    public Guid ShipperId { get; set; }

    public List<Guid> ParcelIds { get; set; } = new();

    public decimal TotalCollected { get; set; } 
    // tổng tiền shipper thu từ khách

    public decimal TotalSubmitted { get; set; } 
    // tiền shipper đã nộp về

    public decimal TotalSettled { get; set; } 
    // tiền đã chuyển cho merchant

    public string Status { get; set; } = "PENDING";
    // PENDING / COMPLETED / FAILED

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? SettledDate { get; set; }
}