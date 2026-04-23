using MediatR;

namespace LWMS.Application.Parcels.Queries.GetByTracking;
public class GetParcelByTrackingQuery : IRequest<ParcelDetailDto>
{
    public string TrackingCode { get; set; } = string.Empty;
}