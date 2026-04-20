using LWMS.Domain.Common;
using LWMS.Domain.Enums;

namespace LWMS.Domain.Entities;
public class TrackingLog : BaseEntity
{
    public Guid ParcelId { get; set; }
    public ParcelStatus FromStatus { get; set; } 
    public ParcelStatus ToStatus { get; set; } 
    public Guid ActorId { get; set; } 
    public string Location { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedTime {get;set;} = DateTime.UtcNow;
}