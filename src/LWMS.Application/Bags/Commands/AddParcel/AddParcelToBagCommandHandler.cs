using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using LWMS.Domain.Entities;
namespace LWMS.Application.Bags.Commands.AddParcel;
public class AddParcelToBagCommandHandler :IRequestHandler<AddParcelToBagCommand,bool>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;

    public AddParcelToBagCommandHandler(IUnitOfWork uow, ICurrentUserService currentUserService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(AddParcelToBagCommand request, CancellationToken cancellationToken)
    {
        // 1. Tìm Bag
        var bag = await _uow.Bags.Query().FirstOrDefaultAsync(x => x.Id == request.BagId, cancellationToken);
        if (bag == null) throw new LWMS.Application.Common.Exceptions.BusinessException("Không tìm thấy bao tải.");

        // 2. Kiểm tra trạng thái bao tải
        if (bag.Status != BagStatus.Open) 
            throw new LWMS.Application.Common.Exceptions.BusinessException("Bao tải đã niêm phong hoặc đã nhận, không thể thêm hàng.");

        // 3. Tìm Parcel
        var parcel = await _uow.Parcels.Query().FirstOrDefaultAsync(x => x.TrackingCode == request.TrackingCode, cancellationToken);
        if (parcel == null) throw new LWMS.Application.Common.Exceptions.BusinessException("Không tìm thấy bưu kiện.");

        // 4. Edge Case: Chặn đơn hàng đã nằm trong bao tải khác
        var existed = await _uow.BagItems.Query()
            .AnyAsync(x => x.ParcelId == parcel.Id, cancellationToken);
        if (existed) throw new LWMS.Application.Common.Exceptions.BusinessException("Bưu kiện này đã nằm trong một bao tải khác rồi!");

        // 5. Check State Machine: Chỉ cho phép đóng bao khi đã Inbound hoặc đã Sort
        if (parcel.Status != ParcelStatus.ArrivedHub && parcel.Status != ParcelStatus.Sorted)
        {
            throw new LWMS.Application.Common.Exceptions.BusinessException($"Trạng thái bưu kiện ({parcel.Status}) không hợp lệ để đóng bao.");
        }

        // 6. Tạo Bag Item
        var item = new BagItem
        {
            BagId = bag.Id,
            ParcelId = parcel.Id,
            AddedAt = DateTime.UtcNow,
            AddedBy = _currentUserService.UserId ?? Guid.Empty
        };
        await _uow.BagItems.AddAsync(item);

        // 7. Update Bag & Parcel
        bag.ParcelCount++;
        var log = parcel.ChangeStatusWithLog(
            ParcelStatus.InBag,
            _currentUserService.UserId ?? Guid.Empty,
            $"Đóng vào bao {bag.BagCode} tại kho {_currentUserService.UserId}"
        );
        await _uow.TrackingLogs.AddAsync(log);

        await _uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}