using MediatR;

namespace LWMS.Application.Parcels.Commands.Sort;
public class SortParcelCommand : IRequest<bool>
{
    public string TrackingCode { get; set; } = string.Empty;
}