using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Entities;
using LWMS.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LWMS.Application.Bags.Commands.ReAssignShipper;

public class ReAssignShipperCommandHandler : IRequestHandler<ReAssignShipperCommand, bool>
{
    private readonly IUnitOfWork _uow;

    public ReAssignShipperCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<bool> Handle(ReAssignShipperCommand request, CancellationToken cancellationToken)
    {
        var parcel = await _uow.Parcels.Query().FirstOrDefaultAsync(x => x.TrackingCode == request.TrackingCode, cancellationToken);
        if (parcel == null) throw new Exception("Parcel not found");

        // Allowed to re-assign if failed delivery but haven't reached max attempts
        if (parcel.Status != ParcelStatus.FailedDelivery)
            throw new Exception("Parcel is not in FailedDelivery status");

        var shipper = await _uow.Users.GetByIdAsync(request.ShipperId);
        if (shipper == null) throw new Exception("Shipper not found");

        var assignment = new ShipperAssignment
        {
            ParcelId = parcel.Id,
            ShipperId = request.ShipperId,
            HubId = parcel.CurrentHubId ?? parcel.DestHubId,
            AssignedAt = DateTime.UtcNow,
            Status = "RE-ASSIGNED"
        };
        await _uow.ShipperAssignments.AddAsync(assignment);

        var log = parcel.ChangeStatusWithLog(
            ParcelStatus.OutForDelivery,
            request.ShipperId,
            "RE-ASSIGNED TO SHIPPER"
        );

        await _uow.TrackingLogs.AddAsync(log);
        await _uow.SaveChangesAsync(cancellationToken);

        return true;
    }
}
