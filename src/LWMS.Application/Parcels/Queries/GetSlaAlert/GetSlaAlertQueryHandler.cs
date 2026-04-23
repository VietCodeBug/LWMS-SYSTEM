using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LWMS.Application.Parcels.Queries.GetSlaAlert;

public class GetSlaAlertQueryHandler : IRequestHandler<GetSlaAlertQuery, List<SlaAlertDto>>
{
    private readonly IUnitOfWork _uow;

    public GetSlaAlertQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<List<SlaAlertDto>> Handle(GetSlaAlertQuery request, CancellationToken cancellationToken)
    {
        var threshold = DateTime.UtcNow.AddHours(request.WarningHours);

        var alerts = await _uow.Parcels.Query()
            .Where(p => p.SlaDate != null 
                        && p.SlaDate <= threshold 
                        && p.Status != ParcelStatus.Delivered 
                        && p.Status != ParcelStatus.Returned 
                        && p.Status != ParcelStatus.Cancelled)
            .OrderBy(p => p.SlaDate)
            .Select(p => new SlaAlertDto
            {
                Id = p.Id,
                TrackingCode = p.TrackingCode,
                SlaDate = p.SlaDate,
                RemainingHours = p.SlaDate.HasValue ? (p.SlaDate.Value - DateTime.UtcNow).TotalHours : 0,
                Status = p.Status
            })
            .ToListAsync(cancellationToken);

        return alerts;
    }
}
