using LWMS.Application.Common.Exceptions;
using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LWMS.Application.Parcels.Commands.ScanInbound;
public class ScanInboundCommandHandler : IRequestHandler<ScanInboundCommand, bool>
{
    private readonly IUnitOfWork _uow;
    public ScanInboundCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }
    public async Task<bool> Handle(ScanInboundCommand request, CancellationToken cancellationToken)
    {
        var parcel = await _uow.Parcels.Query().FirstOrDefaultAsync(x=> x.TrackingCode == request.TrackingCode);

        if(parcel == null)
            throw new BusinessException("Không tìm thấy bưu kiện.");
            
        //RULE: Only allow scanning inbound for parcels that are in "Created" or "LabelPrinted" status
        if(parcel.Status != ParcelStatus.Created && parcel.Status != ParcelStatus.LabelPrinted)
        {
            throw new BusinessException($"Trạng thái bưu kiện {parcel.Status} không hợp lệ để nhập kho.");
        }
        //Update hub hien tai
        parcel.CurrentHubId = request.HubId;
        //Use Domain Method and LOG
        var log = parcel.ChangeStatusWithLog(ParcelStatus.ArrivedHub,Guid.Empty, $"HUB:{request.HubId}");
        await _uow.TrackingLogs.AddAsync(log);
        await _uow.SaveChangesAsync();
        return true;
    }
}