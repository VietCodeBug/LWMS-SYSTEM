using MediatR;
using LWMS.Application.Common.Security;

namespace LWMS.Application.Bags.Commands.DeliverySuccess;

[Authorize(Roles = "Shipper,Admin")]
public class DeliverySuccessCommand : IRequest<bool>
{
    public string TrackingCode { get; set; } = string.Empty;
    public decimal CodAmount { get; set; }
}