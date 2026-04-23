using MediatR;
using Microsoft.EntityFrameworkCore;
using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Enums;

namespace LWMS.Application.Bags.Commands.Receive;

public class ReceiveBagCommandHandler 
    : IRequestHandler<ReceiveBagCommand, bool>
{
    private readonly IUnitOfWork _uow;

    public ReceiveBagCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<bool> Handle(
        ReceiveBagCommand request,
        CancellationToken cancellationToken)
    {
        // 🔥 1. tìm bag
        var bag = await _uow.Bags
            .Query()
            .FirstOrDefaultAsync(x => x.Id == request.BagId);

        if (bag == null)
            throw new Exception("Bag not found");

        // ❌ chỉ nhận khi đã sealed
        if (bag.Status != BagStatus.Sealed)
            throw new Exception("Bag chưa được seal");

        // 🔥 2. lấy danh sách parcel trong bag
        var items = await _uow.BagItems
            .Query()
            .Where(x => x.BagId == bag.Id)
            .ToListAsync();

        // 🔥 3. update trạng thái bag
        bag.Status = BagStatus.Arrived;

        // 🔥 4. update từng parcel
        foreach (var item in items)
        {
            var parcel = await _uow.Parcels.GetByIdAsync(item.ParcelId);
            if (parcel == null) continue;

            var log = parcel.ChangeStatusWithLog(
                ParcelStatus.ArrivedHub,
                Guid.Empty,
                $"ARRIVED AT HUB {bag.ToHubId}"
            );

            await _uow.TrackingLogs.AddAsync(log);
        }

        await _uow.SaveChangesAsync();

        return true;
    }
}