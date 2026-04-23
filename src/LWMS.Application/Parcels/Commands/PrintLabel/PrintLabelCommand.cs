using MediatR;

namespace LWMS.Application.Parcels.Commands.PrintLabel;
public class PrintLabelCommand :IRequest<bool>
{
    public string TrackingCode { get; set; } = string.Empty;
}