using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Entities;
using MediatR;

namespace LWMS.Application.Merchants.Commands.Create;

public class CreateMerchantCommand : IRequest<Guid>
{
    public string MerchantCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class CreateMerchantCommandHandler : IRequestHandler<CreateMerchantCommand, Guid>
{
    private readonly IUnitOfWork _uow;

    public CreateMerchantCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Guid> Handle(CreateMerchantCommand request, CancellationToken cancellationToken)
    {
        var merchant = new Merchant
        {
            MerchantCode = request.MerchantCode,
            Name = request.Name,
            Phone = request.Phone,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Merchants.AddAsync(merchant);
        await _uow.SaveChangesAsync(cancellationToken);

        return merchant.Id;
    }
}
