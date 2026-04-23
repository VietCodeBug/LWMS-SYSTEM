using MediatR;

namespace LWMS.Application.Bags.Commands.AddParcel;
public class AddParcelToBagCommand : IRequest<bool>
{
    public Guid BagId { get; set; }
    public string TrackingCode { get; set; } = string.Empty;
}