using LWMS.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LWMS.Application.Hubs.Queries.GetHubList;

public class HubBriefDto
{
    public Guid Id { get; set; }
    public string HubCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

public class GetHubListQuery : IRequest<List<HubBriefDto>> { }

public class GetHubListQueryHandler : IRequestHandler<GetHubListQuery, List<HubBriefDto>>
{
    private readonly IUnitOfWork _uow;

    public GetHubListQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<List<HubBriefDto>> Handle(GetHubListQuery request, CancellationToken cancellationToken)
    {
        return await _uow.Hubs.Query()
            .OrderBy(h => h.HubCode)
            .Select(h => new HubBriefDto
            {
                Id = h.Id,
                HubCode = h.HubCode,
                Name = h.Name,
                Address = h.Address
            })
            .ToListAsync(cancellationToken);
    }
}
