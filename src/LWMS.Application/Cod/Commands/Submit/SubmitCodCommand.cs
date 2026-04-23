using MediatR;

namespace LWMS.Application.Cod.Commands.Submit;

public class SubmitCodCommand : IRequest<bool>
{
    public string TrackingCode { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
