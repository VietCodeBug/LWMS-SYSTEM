using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Entities;
using LWMS.Domain.Enums;
using MediatR;

namespace LWMS.Application.Bags.Commands.Create;
public class CreateBagCommandHandler :IRequestHandler<CreateBagCommand,Guid>
{
    private readonly IUnitOfWork _uow;
    public CreateBagCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }
    public async Task<Guid> Handle(CreateBagCommand request, CancellationToken cancellationToken)
    {
        //Tao BAGCODE don gian(sau nag cap sau)
        var bagCode = $"BAG-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        var bag = new Bag
        {
            BagCode = bagCode,
            FromHubId = request.FromHubId,
            ToHubId = request.ToHubId,
            Status = BagStatus.Open,//IMPORTANT: Mới tạo thì luôn là Open, sau này khi có cập nhật trạng thái thì sẽ thay đổi
            ParcelCount = 0,
        };
        await _uow.Bags.AddAsync(bag);
        await _uow.SaveChangesAsync();
        return bag.Id;
    }
}