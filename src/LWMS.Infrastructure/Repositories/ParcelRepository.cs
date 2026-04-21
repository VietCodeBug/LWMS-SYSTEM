using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Entities;
using LWMS.Domain.Enums;
using LWMS.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace LWMS.Infrastructure.Repositories;
public class ParcelRepository  : RepositoryBase<Parcel>,IParcelRepository
{
    public ParcelRepository(AppDbContext context) : base(context)
    {
    }
    public async Task<Parcel?> GetByTrackingCodeAsync(string trackingCode)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.TrackingCode == trackingCode);
    }
    public async Task<List<Parcel>> GetByStatusAsync(ParcelStatus status)
    {
        return await _dbSet.Where(p => p.Status == status).ToListAsync();
    }
    public async Task<List<Parcel>> GetByMerchantAsync(Guid merchantId)
    {
        return await _dbSet.Where(p => p.MerchantId == merchantId).ToListAsync();
    }
    public async Task<List<Parcel>> GetSlaAlertAsync(DateTime now)
    {
        return await _dbSet.Where(p => p.SlaDate.HasValue && p.SlaDate.Value < now && p.Status != ParcelStatus.Delivered).ToListAsync();
    }
}