using LWMS.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LWMS.Application.Cod.Commands.Settle;

public class SettleCodCommandHandler : IRequestHandler<SettleCodCommand, bool>
{
    private readonly IUnitOfWork _uow;

    public SettleCodCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<bool> Handle(SettleCodCommand request, CancellationToken cancellationToken)
    {
        var parcel = await _uow.Parcels.Query().FirstOrDefaultAsync(x => x.TrackingCode == request.TrackingCode, cancellationToken);
        if (parcel == null) throw new Exception("Parcel not found");

        var codRecord = await _uow.CodRecords.Query().FirstOrDefaultAsync(x => x.ParcelId == parcel.Id && x.Status == "SUBMITTED", cancellationToken);
        if (codRecord == null) throw new Exception("No submitted COD record found for this parcel");

        codRecord.Status = "SETTLED";
        codRecord.SettledAt = DateTime.UtcNow;

        await _uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}
