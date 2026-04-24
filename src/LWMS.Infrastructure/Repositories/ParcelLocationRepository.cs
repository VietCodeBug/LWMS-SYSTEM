using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Entities;
using LWMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LWMS.Infrastructure.Repositories;

public class ParcelLocationRepository : RepositoryBase<ParcelLocation>, IParcelLocationRepository
{
    public ParcelLocationRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<ParcelLocation?> GetActiveLocationByParcelIdAsync(Guid parcelId)
    {
        return await _dbSet
            .Where(pl => pl.ParcelId == parcelId && pl.OutDate == null)
            .OrderByDescending(pl => pl.InDate)
            .FirstOrDefaultAsync();
    }

    public async Task<List<ParcelLocation>> GetHistoryByParcelIdAsync(Guid parcelId)
    {
        return await _dbSet
            .Where(pl => pl.ParcelId == parcelId)
            .OrderByDescending(pl => pl.InDate)
            .ToListAsync();
    }
}
