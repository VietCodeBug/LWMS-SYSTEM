using LWMS.Domain.Common;
using LWMS.Domain.Entities;
public class Bag: BaseEntity
{
    public string BagCode {get;set;} = string.Empty;
    public Guid FromHubId {get;set;}
    public Guid ToHubId {get;set;}
    public DateTime CreateDateTime {get;set;} = DateTime.UtcNow;
    public List<BagItem> Packages {get;set;} = new();
}
