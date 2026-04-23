using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Entities;
using MediatR;

namespace LWMS.Application.Hubs.Commands.Create;

public class CreateHubCommand : IRequest<Guid>
{
    public string HubCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ProvinceCode { get; set; } = string.Empty;
    public string HubType { get; set; } = "STATION";
}

public class CreateHubCommandHandler : IRequestHandler<CreateHubCommand, Guid>
{
    private readonly IUnitOfWork _uow;

    public CreateHubCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Guid> Handle(CreateHubCommand request, CancellationToken cancellationToken)
    {
        var hub = new Hub
        {
            HubCode = request.HubCode,
            Name = request.Name,
            Address = request.Address,
            ProvinceCode = request.ProvinceCode,
            HubType = request.HubType,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Hubs.AddAsync(hub);
        await _uow.SaveChangesAsync(cancellationToken);

        return hub.Id;
    }
}
