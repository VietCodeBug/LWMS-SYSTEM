using LWMS.Domain.Common;

namespace LWMS.Domain.Entities;
public class AuditLog : BaseEntity
{
    public Guid ActorId { get; set; }//Id của người thực hiện hành động
    public string ActorRole { get; set; } = string.Empty;//Admin, Staff, System
    public string Action { get; set; } = string.Empty;//CREATE, UPDATE, DELETE
    public string EntityType { get; set; } = string.Empty;//Parcel,User,...

    public Guid EntityId { get; set; }//Id Bị Oject tác dụng
    public string? Changes {get;set;}
    //Json:{"status":"CREATED->DELIVERED"}
    public string? IpAddress { get; set; }
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

}