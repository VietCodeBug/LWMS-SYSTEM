using LWMS.Domain.Common;

namespace LWMS.Domain.Entities;

public class ShipperAssignment : BaseEntity
{
    public Guid ParcelId { get; set; }

    public Guid ShipperId { get; set; }

    public Guid HubId { get; set; }

    public string Type { get; set; } = "DELIVERY";
    // DELIVERY / PICKUP

    public string Status { get; set; } = "ASSIGNED";
    // ASSIGNED / IN_PROGRESS / DONE

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}