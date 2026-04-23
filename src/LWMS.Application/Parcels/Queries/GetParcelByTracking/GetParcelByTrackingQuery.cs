using MediatR;

namespace LWMS.Application.Parcels.Queries.GetParcelByTracking;

public class GetParcelByTrackingQuery : IRequest<ParcelDto?>
{
    public string TrackingCode { get; set; } = string.Empty;
}
