using MediatR;

namespace LWMS.Application.Bags.Commands.Create;
public class CreateBagCommand :IRequest<Guid>
{
    public Guid FromHubId { get; set; }
    public Guid ToHubId { get; set; }
}