using MediatR;

namespace LWMS.Application.Bags.Commands.Receive;

public class ReceiveBagCommand : IRequest<bool>
{
    public Guid BagId { get; set; }
}