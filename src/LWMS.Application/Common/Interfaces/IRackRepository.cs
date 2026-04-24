using LWMS.Domain.Entities;

namespace LWMS.Application.Common.Interfaces;

public interface IRackRepository : IRepository<Rack>
{
    Task<Rack?> GetByCodeAsync(string code);
    Task<List<Rack>> GetByHubAsync(Guid hubId);
}
