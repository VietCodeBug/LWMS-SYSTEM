using MediatR;
using LWMS.Application.Common.Security;

namespace LWMS.Application.Bags.Commands.AssignShipper;

[Authorize(Roles = "Admin,HubStaff")]
public class AssignShipperCommand : IRequest<bool>
{
    public string TrackingCode { get; set; } = string.Empty;
    public Guid ShipperId { get; set; }
}
