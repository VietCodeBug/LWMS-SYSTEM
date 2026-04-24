using MediatR;
using LWMS.Domain.Entities;
using LWMS.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LWMS.Application.Warehouse.Commands.Picking;

public record PickingCommand : IRequest<bool>
{
    public string TrackingCode { get; init; } = string.Empty;
    public Guid ActorId { get; init; }
}

public class PickingCommandHandler : IRequestHandler<PickingCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public PickingCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(PickingCommand request, CancellationToken cancellationToken)
    {
        var parcel = await _unitOfWork.Parcels.GetByTrackingCodeAsync(request.TrackingCode);
        if (parcel == null) throw new KeyNotFoundException("Không tìm thấy bưu kiện.");

        // Tìm vị trí hiện tại đang ở trên kệ (Movement IN và OutDate null)
        var currentLocation = await _unitOfWork.ParcelLocations.GetActiveLocationByParcelIdAsync(parcel.Id);
        
        if (currentLocation == null)
        {
            throw new InvalidOperationException("Bưu kiện không nằm trên kệ nào để lấy ra.");
        }

        var rack = await _unitOfWork.Racks.GetByIdAsync(currentLocation.RackId);
        if (rack != null)
        {
            rack.CurrentUsage = Math.Max(0, rack.CurrentUsage - 1);
        }

        // Cập nhật OutDate cho vị trí cũ
        currentLocation.OutDate = DateTime.UtcNow;
        currentLocation.MovementType = "OUT"; // Có thể dùng OUT để đánh dấu đã lấy ra

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
