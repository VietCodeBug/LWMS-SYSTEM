using MediatR;

namespace LWMS.Application.Parcels.Commands.Camcel;
public class CancelParcelCommand : IRequest<bool>
{
    public string TrackingCode { get; set; } = string.Empty;
}