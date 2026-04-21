namespace LWMS.Domain.Common;

/// <summary>
/// 🏢 Interface đánh dấu Entity thuộc sở hữu của một Merchant cụ thể.
/// Dùng để ép bộ lọc Global Query Filter (Multi-tenancy).
/// </summary>
public interface IMustHaveMerchant
{
    public Guid MerchantId { get; set; }
}
