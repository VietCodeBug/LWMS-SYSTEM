using MediatR;

namespace LWMS.Application.Bags.Commands.DeliveryFailed;

public class DeliveryFailedCommand : IRequest<bool>
{
    public string TrackingCode { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}