using LWMS.Domain.Common;

namespace LWMS.Domain.Entities;
public class CodRecord : BaseEntity{
    public Guid ParcelId {get;set;}
    public decimal Amount {get;set;}
    public bool IsCollected {get;set;}
}