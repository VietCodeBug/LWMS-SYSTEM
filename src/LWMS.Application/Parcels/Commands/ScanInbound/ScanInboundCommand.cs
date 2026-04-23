using MediatR;

namespace LWMS.Application.Parcels.Commands.ScanInbound;
public class ScanInboundCommand :IRequest<bool>
{
    public string TrackingCode { get; set; } = string.Empty;
    public Guid HubId { get; set; } // Warehouse/Hub where the parcel is scanned in
    public string? Note { get; set; }
}