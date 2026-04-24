using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Entities;
using LWMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LWMS.Infrastructure.Repositories;

public class RackRepository : RepositoryBase<Rack>, IRackRepository
{
    public RackRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Rack?> GetByCodeAsync(string code)
    {
        return await _dbSet.FirstOrDefaultAsync(r => r.RackCode == code);
    }

    public async Task<List<Rack>> GetByHubAsync(Guid hubId)
    {
        return await _dbSet.Where(r => r.HubId == hubId).ToListAsync();
    }
}
