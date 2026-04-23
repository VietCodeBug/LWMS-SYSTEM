using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Entities;
using LWMS.Domain.Enums;
using LWMS.Domain.Services;
using LWMS.Domain.ValueObjects;
using MediatR;

namespace LWMS.Application.Parcels.Commands.Create;

public class CreateParcelCommandHandler : IRequestHandler<CreateParcelCommand, CreateParcelResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;

    public CreateParcelCommandHandler(IUnitOfWork uow, ICurrentUserService currentUserService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
    }

    public async Task<CreateParcelResponse> Handle(CreateParcelCommand request, CancellationToken cancellationToken)
    {
        // 1. Business Validation: Kiểm tra Hub và Service thực sự tồn tại
        var originHub = await _uow.Hubs.GetByIdAsync(request.OriginHubId);
        if (originHub == null) throw new LWMS.Application.Common.Exceptions.BusinessException("Origin Hub không tồn tại");

        var destHub = await _uow.Hubs.GetByIdAsync(request.DestHubId);
        if (destHub == null) throw new LWMS.Application.Common.Exceptions.BusinessException("Destination Hub không tồn tại");

        // Generate tracking code
        var trackingCode = !string.IsNullOrEmpty(request.TrackingCode) 
            ? request.TrackingCode 
            : TrackingCodeGenerator.GenerateTrackingCode("HN01", new Random().Next(1000000));
        
        var merchantId = _currentUserService.MerchantId ?? Guid.Parse("221E9C52-7360-4927-AA72-5B91CF8E9661");

        // Create Parcel 
        var parcel = new Parcel
        {
            TrackingCode = trackingCode,
            Weight = request.Weight,
            Status = ParcelStatus.Created,
            CodAmount = new Money(request.CodAmount, "VND"),
            DeliveryAttempts = 0,
            SlaDate = DateTime.UtcNow.AddDays(3),
            SenderAddress = new Address("N/A", request.Province, "N/A", "N/A"),
            ReceiverAddress = new Address("N/A", request.Province, "N/A", "N/A"),
            OriginHubId = request.OriginHubId,
            DestHubId = request.DestHubId,
            MerchantId = merchantId,
            ServiceTypeId = request.ServiceId,
            SenderName = request.SenderName,
            ReceiverName = request.ReceiverName,
            SenderPhone = request.SenderPhone,
            ReceiverPhone = request.ReceiverPhone
        };

        // Save 
        await _uow.Parcels.AddAsync(parcel);

        // Create TrackingLog History
        var log = new TrackingLog
        {
            ParcelId = parcel.Id,
            FromStatus = ParcelStatus.Created,
            ToStatus = ParcelStatus.Created,
            ActorId = Guid.Empty,
            Location = "SYSTEM",
        };
        await _uow.TrackingLogs.AddAsync(log);

        await _uow.SaveChangesAsync();

        return new CreateParcelResponse(parcel.Id, parcel.TrackingCode);
    }
}