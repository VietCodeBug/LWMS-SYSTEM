using LWMS.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LWMS.Application.Parcels.Queries.GetByTracking;
public class GetParcelByTrackingHandler : IRequestHandler<GetParcelByTrackingQuery, ParcelDetailDto>
{
    private readonly IUnitOfWork _uow;
    public GetParcelByTrackingHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }
    public async Task<ParcelDetailDto>Handle(GetParcelByTrackingQuery request,CancellationToken cancellationToken)
    {
        var parcel = await _uow.Parcels.Query().AsNoTracking().FirstOrDefaultAsync(x => x.TrackingCode == request.TrackingCode);
        if (parcel == null)
            throw new Exception("Parcel not found");
            return new ParcelDetailDto
        {
            TrackingCode = parcel.TrackingCode,
            Status = parcel.Status.ToString(),

            SenderName = parcel.SenderName,
            SenderPhone = MaskPhone(parcel.SenderPhone),

            ReceiverName = parcel.ReceiverName,
            ReceiverPhone = MaskPhone(parcel.ReceiverPhone),

            Weight = parcel.Weight,
            SlaDate = parcel.SlaDate,

            TrackingLogs = parcel.TrackingLogs
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new TrackingLogDto
                {
                    FromStatus = x.FromStatus.ToString(),
                    ToStatus = x.ToStatus.ToString(),
                    Location = x.Location,
                    CreatedAt = x.CreatedAt
                }).ToList()
        };
    }

    private string MaskPhone(string phone)
    {
        if (string.IsNullOrEmpty(phone) || phone.Length < 6)
            return phone;

        return phone[..3] + "****" + phone[^3..];
    }
}