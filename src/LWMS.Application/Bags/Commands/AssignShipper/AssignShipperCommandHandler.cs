using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Entities;
using LWMS.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LWMS.Application.Bags.Commands.AssignShipper;
public class AssignShipperCommandHandler : IRequestHandler<AssignShipperCommand, bool>
{
    private readonly IUnitOfWork _uow;
    public AssignShipperCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }
    public async Task<bool> Handle(AssignShipperCommand request, CancellationToken cancellationToken)
    {
        // 1. Tìm Parcel
        var parcel = await _uow.Parcels.Query().FirstOrDefaultAsync(x => x.TrackingCode == request.TrackingCode, cancellationToken);
        if (parcel == null) throw new Exception("Parcel not Found");

        // Chỉ assign khi đã tới kho đích
        if (parcel.Status != ParcelStatus.ArrivedHub) throw new Exception("Parcel chưa tới Kho đích");
        if (parcel.CurrentHubId == null)
        throw new Exception("Parcel chưa có CurrentHubId");
        // 2. Check xem có tồn tại shipper
        var shipper = await _uow.Users.GetByIdAsync(request.ShipperId);
        if (shipper == null) throw new Exception("Shipper not found");

        // 3. Tạo assignment
        var assignment = new ShipperAssignment
        {
    
            ParcelId = parcel.Id,
            ShipperId = request.ShipperId,
            HubId = parcel.CurrentHubId.Value,
            AssignedAt = DateTime.UtcNow,
            Status = "ASSIGNED"
        };
        await _uow.ShipperAssignments.AddAsync(assignment);

        // 4. Update parcel + log
        var log = parcel.ChangeStatusWithLog(
            ParcelStatus.OutForDelivery,
            request.ShipperId,
            "ASSIGNED TO SHIPPER"
        );

        await _uow.TrackingLogs.AddAsync(log);

        await _uow.SaveChangesAsync(cancellationToken);

        return true;
    }
}