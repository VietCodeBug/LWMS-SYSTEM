using LWMS.Domain.Common;
using LWMS.Domain.Enums;
using LWMS.Domain.ValueObjects;
using LWMS.Domain.Services;

namespace LWMS.Domain.Entities;

public class Parcel : BaseEntity
{
    public string TrackingCode { get; set; } = string.Empty;

    public Address SenderAddress { get; set; } = null!;

    public Address ReceiverAddress { get; set; } = null!;

    public decimal Weight { get; set; }

    public Money CodAmount { get; set; } = null!;

    public ParcelStatus Status { get; set; } = ParcelStatus.Created;

    public int FailCount { get; set; } = 0;

    public DateTime? SlaDate { get; set; }

    // ✅ METHOD PHẢI NẰM TRONG CLASS
    public void ChangeStatus(ParcelStatus newStatus)
    {
        if (!ParcelStateMachine.CanTransition(Status, newStatus))
        {
            throw new InvalidOperationException(
                $"Cannot transition from {Status} to {newStatus}"
            );
        }

        Status = newStatus; // ✅ QUAN TRỌNG
    }

    //Method TrackingLog
    public TrackingLog ChangeStatusWithLog(ParcelStatus newStatus,Guid actorId,string location)
    {
        if (!ParcelStateMachine.CanTransition(Status, newStatus))
        {
            throw new InvalidOperationException($"Cannot transition from{Status} to {newStatus}");
        }
        var log = new TrackingLog
        {
            ParcelId = this.Id,
            FromStatus = this.Status,
            ToStatus = newStatus,
            ActorId = actorId,
            Location = location,
           
        };
        Status = newStatus;
        return log;
    }
}