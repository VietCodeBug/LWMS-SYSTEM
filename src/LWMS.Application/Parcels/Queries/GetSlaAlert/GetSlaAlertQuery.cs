using LWMS.Domain.Enums;
using MediatR;

namespace LWMS.Application.Parcels.Queries.GetSlaAlert;

public class SlaAlertDto
{
    public Guid Id { get; set; }
    public string TrackingCode { get; set; } = string.Empty;
    public DateTime? SlaDate { get; set; }
    public double RemainingHours { get; set; }
    public ParcelStatus Status { get; set; }
}

public class GetSlaAlertQuery : IRequest<List<SlaAlertDto>>
{
    public int WarningHours { get; set; } = 24;
}
