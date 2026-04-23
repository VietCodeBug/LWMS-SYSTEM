using LWMS.Domain.Enums;
using MediatR;

namespace LWMS.Application.Parcels.Queries.GetParcelList;

public class GetParcelListQuery : IRequest<PaginatedList<ParcelBriefDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public ParcelStatus? Status { get; set; }
}
