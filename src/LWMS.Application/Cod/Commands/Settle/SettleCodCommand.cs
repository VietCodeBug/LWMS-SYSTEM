using MediatR;

namespace LWMS.Application.Cod.Commands.Settle;

public class SettleCodCommand : IRequest<bool>
{
    public string TrackingCode { get; set; } = string.Empty;
}
