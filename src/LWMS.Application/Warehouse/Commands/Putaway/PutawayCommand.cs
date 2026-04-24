using MediatR;
using LWMS.Domain.Entities;
using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Enums;

namespace LWMS.Application.Warehouse.Commands.Putaway;

public record PutawayCommand : IRequest<bool>
{
    public string TrackingCode { get; init; } = string.Empty;
    public string RackCode { get; init; } = string.Empty;
    public Guid ActorId { get; init; }
    public string? Note { get; init; }
}

public class PutawayCommandHandler : IRequestHandler<PutawayCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public PutawayCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(PutawayCommand request, CancellationToken cancellationToken)
    {
        var parcel = await _unitOfWork.Parcels.GetByTrackingCodeAsync(request.TrackingCode);
        if (parcel == null) throw new KeyNotFoundException("Không tìm thấy bưu kiện.");

        // Bưu kiện phải ở Hub (ArrivedHub hoặc Sorted hoặc FailedDelivery)
        if (parcel.Status != ParcelStatus.ArrivedHub && 
            parcel.Status != ParcelStatus.Sorted && 
            parcel.Status != ParcelStatus.FailedDelivery)
        {
            throw new InvalidOperationException($"Không thể đưa bưu kiện vào kệ khi đang ở trạng thái {parcel.Status}.");
        }

        var rack = await _unitOfWork.Racks.GetByCodeAsync(request.RackCode);
        if (rack == null) throw new KeyNotFoundException("Không tìm thấy kệ kho.");
        
        if (rack.IsLocked || rack.IsFull)
        {
            throw new InvalidOperationException("Kệ đã đầy hoặc đang bị khóa.");
        }

        // Tạo vị trí mới (Movement IN)
        var location = new ParcelLocation
        {
            ParcelId = parcel.Id,
            RackId = rack.Id,
            ActorId = request.ActorId,
            MovementType = "IN",
            InDate = DateTime.UtcNow,
            Note = request.Note ?? string.Empty
        };

        rack.CurrentUsage++;
        
        await _unitOfWork.ParcelLocations.AddAsync(location);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
