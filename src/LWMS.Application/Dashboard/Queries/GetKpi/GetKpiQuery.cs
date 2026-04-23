using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LWMS.Application.Dashboard.Queries.GetKpi;

public class KpiDto
{
    public int TotalParcels { get; set; }
    public int DeliveredToday { get; set; }
    public int PendingPickup { get; set; }
    public int FailedToday { get; set; }
    public decimal TotalCodCollected { get; set; }
}

public class GetKpiQuery : IRequest<KpiDto> { }

public class GetKpiQueryHandler : IRequestHandler<GetKpiQuery, KpiDto>
{
    private readonly IUnitOfWork _uow;

    public GetKpiQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<KpiDto> Handle(GetKpiQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;

        return new KpiDto
        {
            TotalParcels = await _uow.Parcels.Query().CountAsync(cancellationToken),
            DeliveredToday = await _uow.Parcels.Query().CountAsync(p => p.Status == ParcelStatus.Delivered && p.UpdatedAt >= today, cancellationToken),
            PendingPickup = await _uow.Parcels.Query().CountAsync(p => p.Status == ParcelStatus.Created, cancellationToken),
            FailedToday = await _uow.Parcels.Query().CountAsync(p => p.Status == ParcelStatus.FailedDelivery && p.UpdatedAt >= today, cancellationToken),
            TotalCodCollected = await _uow.CodRecords.Query().Where(c => c.Status == "COLLECTED").SumAsync(c => c.Amount, cancellationToken)
        };
    }
}
