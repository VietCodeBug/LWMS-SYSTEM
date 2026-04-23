using MediatR;

namespace LWMS.Application.Bags.Commands.ReAssignShipper;

public class ReAssignShipperCommand : IRequest<bool>
{
    public string TrackingCode { get; set; } = string.Empty;
    public Guid ShipperId { get; set; }
}
