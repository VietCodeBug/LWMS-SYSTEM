using LWMS.Domain.Common;

namespace LWMS.Domain.Entities;
public class PickupRequest : BaseEntity, IMustHaveMerchant
{
    public Guid MerchantId { get; set; }
    public string Address { get; set; } = string.Empty;
    public DateTime PickupDate { get; set; }

    public Merchant Merchant { get; set; } = null!;
}