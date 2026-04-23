using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LWMS.Application.Bags.Commands.Seal;
public class SealBagCommandHandler :IRequestHandler<SealBagCommand,bool>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;

    public SealBagCommandHandler(IUnitOfWork uow, ICurrentUserService currentUserService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(SealBagCommand request, CancellationToken cancellationToken)
    {
        // 1. Tìm Bag
        var bag = await _uow.Bags.Query().FirstOrDefaultAsync(x => x.Id == request.BagId, cancellationToken);
        if (bag == null) throw new LWMS.Application.Common.Exceptions.BusinessException("Không tìm thấy bao tải.");

        // 2. Kiểm tra trạng thái
        if (bag.Status != BagStatus.Open) 
            throw new LWMS.Application.Common.Exceptions.BusinessException("Bao tải này không ở trạng thái Mở để Niêm phong.");

        if (bag.ParcelCount == 0) 
            throw new LWMS.Application.Common.Exceptions.BusinessException("Bao tải rỗng, không thể niêm phong.");
        
        bag.SealNumber = $"SEAL-{DateTime.UtcNow.Ticks}";
        //2:Lay tat ca parcel trong bag
        var items = await _uow.BagItems.Query().Where(x=>x.BagId==bag.Id).ToListAsync();
        //3:Update trang thai Bag
        bag.Status = BagStatus.Sealed;
        bag.SealedAt = DateTime.UtcNow;
        //4:Update voi tung parcel + log
        foreach (var item in items)
        {
            var parcel = await _uow.Parcels.GetByIdAsync(item.ParcelId);
            if (parcel == null) continue;

            var log = parcel.ChangeStatusWithLog(
                ParcelStatus.InTransit,
                _currentUserService.UserId ?? Guid.Empty,
                $"NIÊM PHONG BAO {bag.BagCode} - SẴN SÀNG VẬN CHUYỂN"
            );
            await _uow.TrackingLogs.AddAsync(log);
        }
         await _uow.SaveChangesAsync();
         return true;
    }
}