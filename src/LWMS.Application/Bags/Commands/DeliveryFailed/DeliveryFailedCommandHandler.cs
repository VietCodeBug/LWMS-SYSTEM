using MediatR;
using Microsoft.EntityFrameworkCore;
using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Enums;
using LWMS.Domain.Entities;

namespace LWMS.Application.Bags.Commands.DeliveryFailed;

public class DeliveryFailedCommandHandler
    : IRequestHandler<DeliveryFailedCommand, bool>
{
    private readonly IUnitOfWork _uow;

    public DeliveryFailedCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<bool> Handle(
        DeliveryFailedCommand request,
        CancellationToken cancellationToken)
    {
        // 🔥 1. tìm parcel
        var parcel = await _uow.Parcels
            .Query()
            .FirstOrDefaultAsync(
                x => x.TrackingCode == request.TrackingCode,
                cancellationToken
            );

        if (parcel == null)
            throw new Exception("Parcel not found");

        // ❌ chỉ fail khi đang giao
        if (parcel.Status != ParcelStatus.OutForDelivery)
            throw new Exception("Parcel không ở trạng thái giao");

        // 🔥 2. tăng số lần fail
        parcel.DeliveryAttempts++;

        // 🔥 3. xử lý theo số lần fail
        if (parcel.DeliveryAttempts >= 3)
        {
            // 👉 tạo return order
            var returnOrder = new ReturnOrder
            {
                Id = Guid.NewGuid(),
                ParcelId = parcel.Id,
                ReturnTrackingCode = "R-" + parcel.TrackingCode,
                Reason = ReturnReason.Other, // Mặc định là Other, có thể map từ request.Reason nếu cần
                Notes = request.Reason,
                Status = "PENDING"
            };

            await _uow.ReturnOrders.AddAsync(returnOrder);

            // 👉 update status + log
            var log = parcel.ChangeStatusWithLog(
                ParcelStatus.Returning,
                Guid.Empty,
                $"FAILED {parcel.DeliveryAttempts} TIMES → RETURN (Lý do: {request.Reason})"
            );

            await _uow.TrackingLogs.AddAsync(log);
        }
        else
        {
            // 👉 fail bình thường
            var log = parcel.ChangeStatusWithLog(
                ParcelStatus.FailedDelivery,
                Guid.Empty,
                request.Reason
            );

            await _uow.TrackingLogs.AddAsync(log);
        }

        // 🔥 4. save
        await _uow.SaveChangesAsync(cancellationToken);

        return true;
    }
}