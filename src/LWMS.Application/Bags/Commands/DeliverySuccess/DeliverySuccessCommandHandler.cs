using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Entities;
using LWMS.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LWMS.Application.Bags.Commands.DeliverySuccess;
public class DeliverySuccessCommandHandler : IRequestHandler<DeliverySuccessCommand, bool>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;

    public DeliverySuccessCommandHandler(IUnitOfWork uow, ICurrentUserService currentUserService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(DeliverySuccessCommand request, CancellationToken cancellationToken)
    {
        var parcel = await _uow.Parcels.Query().FirstOrDefaultAsync(x => x.TrackingCode == request.TrackingCode);
        if (parcel == null) throw new LWMS.Application.Common.Exceptions.BusinessException("Parcel not Found");

        // Idempotency: Nếu đã giao rồi thì không làm gì nữa
        if (parcel.Status == ParcelStatus.Delivered) return true;

        // 2. Security: Kiểm tra xem Shipper này có được gán đơn này không
        if (_currentUserService.Role == "Shipper")
        {
            var assignment = await _uow.ShipperAssignments.Query()
                .Where(sa => sa.ParcelId == parcel.Id && sa.Status == "ASSIGNED")
                .OrderByDescending(sa => sa.CreatedAt)
                .FirstOrDefaultAsync();

            if (assignment == null || assignment.ShipperId != _currentUserService.UserId)
            {
                throw new LWMS.Application.Common.Behaviors.ForbiddenAccessException("Bạn không được gán giao đơn hàng này.");
            }
        }

        // Chỉ khi đang giao
        if (parcel.Status != ParcelStatus.OutForDelivery) 
            throw new LWMS.Application.Common.Exceptions.BusinessException("Parcel khong o trang thai giao hang");
        //COD (Optional)
        if(request.CodAmount > 0)
        {
            var cod = new CodRecord
            {
                ParcelId = parcel.Id,
                Amount = request.CodAmount,
                Status = "COLLECTED",
                CollectedAt = DateTime.UtcNow,
                CollectedBy = _currentUserService.UserId
            };
            await _uow.CodRecords.AddAsync(cod);
        }
        //reset fail count
        parcel.DeliveryAttempts = 0;
        //Update status +log
        var log = parcel.ChangeStatusWithLog(ParcelStatus.Delivered, _currentUserService.UserId ?? Guid.Empty, "DELIVERY SUCCESS");
        await _uow.TrackingLogs.AddAsync(log);
        await _uow.SaveChangesAsync();
        return true;
    }
}