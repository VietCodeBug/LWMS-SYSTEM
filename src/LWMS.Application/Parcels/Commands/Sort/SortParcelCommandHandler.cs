using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LWMS.Application.Parcels.Commands.Sort;
public class SortParcelCommandHandler :IRequestHandler<SortParcelCommand, bool>
{
    private readonly IUnitOfWork _uwo;
    public SortParcelCommandHandler(IUnitOfWork uwo)
    {
        _uwo = uwo;
    }
    public async Task<bool> Handle(SortParcelCommand request, CancellationToken cancellationToken)
    {
        var parcel = await _uwo.Parcels.Query().FirstOrDefaultAsync(p => p.TrackingCode == request.TrackingCode);
        if (parcel == null) throw new Exception("Parcel not found");
        //RULE:Phai co trong kho moi duoc sort
        if(parcel.Status != ParcelStatus.ArrivedHub) throw new Exception("Parcel is not in the hub");
        //SIMPLE ROUTING (tam thoi)
        var desHubId = parcel.DestHubId;
        if(desHubId == Guid.Empty) throw new Exception("Parcel does not have a destination hub");
        //Update Hub Hien Tai(optional)
        parcel.CurrentHubId = parcel.CurrentHubId;
        //Update Hub status lg
        var log = parcel.ChangeStatusWithLog(ParcelStatus.InTransit,Guid.Empty,$"SORTED -> DESTHUB:{desHubId}");
        await _uwo.TrackingLogs.AddAsync(log);
        await _uwo.SaveChangesAsync(cancellationToken);
        return true;
    }
}