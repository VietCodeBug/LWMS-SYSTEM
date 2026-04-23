using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Entities;
using LWMS.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LWMS.Application.Parcels.Commands.Camcel;

public class CancelParcelCommandHandler : IRequestHandler<CancelParcelCommand, bool>
{
    private readonly IUnitOfWork _uow;
    public CancelParcelCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }
    public async Task<bool> Handle(CancelParcelCommand request, CancellationToken cancellationToken)
    {
        var parcel = await _uow.Parcels.Query().FirstOrDefaultAsync(x => x.TrackingCode == request.TrackingCode);
        if (parcel == null)
            throw new Exception("Parcel not found");
        //RUle: Only can cancel when parcel is in pending status
        if (parcel.Status != ParcelStatus.Created && parcel.Status != ParcelStatus.LabelPrinted)
        {
            throw new Exception("Không thể hủy đơn hàng đã được xử lý");
        }
        //Dung Domain Method(Khong set trực tiếp) để thay đổi trạng thái
        var oldStatus = parcel.Status;
        parcel.ChangeStatus(ParcelStatus.Cancelled);
        var log = new TrackingLog
        {
            ParcelId = parcel.Id,
            FromStatus = oldStatus,
            ToStatus = ParcelStatus.Cancelled,
            ActorId = Guid.Empty, //TODO: Get current user id
            Location = "System",
        };
        await _uow.TrackingLogs.AddAsync(log);
        await _uow.SaveChangesAsync();
        return true;
    }
}