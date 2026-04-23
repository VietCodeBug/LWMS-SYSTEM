using LWMS.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LWMS.Application.Parcels.Queries.GetParcelList;

public class GetParcelListQueryHandler : IRequestHandler<GetParcelListQuery, PaginatedList<ParcelBriefDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;

    public GetParcelListQueryHandler(IUnitOfWork uow, ICurrentUserService currentUserService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
    }

    public async Task<PaginatedList<ParcelBriefDto>> Handle(GetParcelListQuery request, CancellationToken cancellationToken)
    {
        var query = _uow.Parcels.Query();

        // 🔥 Ownership Check (Bảo mật dữ liệu Merchant & Shipper)
        if (_currentUserService.Role == "Merchant")
        {
            var merchantId = _currentUserService.MerchantId;
            query = query.Where(p => p.MerchantId == merchantId);
        }
        else if (_currentUserService.Role == "Shipper")
        {
            var userId = _currentUserService.UserId;
            // Chỉ lấy các đơn mà shipper này đang được gán (Assigned/OutForDelivery)
            var assignedParcelIds = _uow.ShipperAssignments.Query()
                .Where(sa => sa.ShipperId == userId && sa.Status == "ASSIGNED")
                .Select(sa => sa.ParcelId);
            
            query = query.Where(p => assignedParcelIds.Contains(p.Id));
        }

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        var count = await query.CountAsync(cancellationToken);
        
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new ParcelBriefDto
            {
                Id = x.Id,
                TrackingCode = x.TrackingCode,
                Status = x.Status,
                ReceiverName = x.ReceiverName,
                Weight = x.Weight,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PaginatedList<ParcelBriefDto>(items, count, request.PageNumber, request.PageSize);
    }
}
