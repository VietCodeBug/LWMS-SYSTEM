using LWMS.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LWMS.Application.Parcels.Queries.GetParcelByTracking;

public class GetParcelByTrackingQueryHandler : IRequestHandler<GetParcelByTrackingQuery, ParcelDto?>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;

    public GetParcelByTrackingQueryHandler(IUnitOfWork uow, ICurrentUserService currentUserService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
    }

    public async Task<ParcelDto?> Handle(GetParcelByTrackingQuery request, CancellationToken cancellationToken)
    {
        var parcel = await _uow.Parcels.Query()
            .Include(x => x.TrackingLogs)
            .FirstOrDefaultAsync(x => x.TrackingCode == request.TrackingCode, cancellationToken);

        if (parcel == null) return null;

        // 🔥 Ownership Check
        if (_currentUserService.Role == "Merchant" && parcel.MerchantId != _currentUserService.MerchantId)
        {
            return null; // Trả về null để API bắn 404, không lộ sự tồn tại của đơn hàng khác
        }

        if (_currentUserService.Role == "Shipper")
        {
            var isAssigned = await _uow.ShipperAssignments.Query()
                .AnyAsync(sa => sa.ParcelId == parcel.Id && sa.ShipperId == _currentUserService.UserId, cancellationToken);
            
            if (!isAssigned) return null;
        }

        return new ParcelDto
        {
            Id = parcel.Id,
            TrackingCode = parcel.TrackingCode,
            Status = parcel.Status,
            SenderName = parcel.SenderName,
            ReceiverName = parcel.ReceiverName,
            ReceiverPhone = parcel.ReceiverPhone,
            Weight = parcel.Weight,
            CodAmount = parcel.CodAmount.Amount,
            TrackingHistory = parcel.TrackingLogs
                .OrderBy(l => l.CreatedAt)
                .Select(l => new TrackingLogDto
                {
                    ToStatus = l.ToStatus,
                    Location = l.Location,
                    CreatedAt = l.CreatedAt
                }).ToList()
        };
    }
}
