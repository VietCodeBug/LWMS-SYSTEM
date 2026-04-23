using MediatR;

namespace LWMS.Application.Bags.Commands.Seal;
public class SealBagCommand :IRequest<bool>
{
    public Guid BagId {get;set;}
}