using LWMS.Domain.Common;

namespace LWMS.Domain.Entities;
public class Merchant :BaseEntity
{
    public string MerchantCode {get;set;} = string.Empty;
    public string Name {get;set;} = string.Empty;
    public string Phone {get;set;} = string.Empty;
    public string Email {get;set;} = string.Empty;
    public Guid? DefaultHubId {get;set;}
    public string ApiKey {get;set;} = string.Empty;
    public bool IsActive {get;set;} = true;
    public decimal BaseFeeMultiplier { get; set; } = 1.0m;

}