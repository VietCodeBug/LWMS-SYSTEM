using LWMS.Domain.Entities;

namespace LWMS.Application.Common.Interfaces;

public interface IParcelLocationRepository : IRepository<ParcelLocation>
{
    Task<ParcelLocation?> GetActiveLocationByParcelIdAsync(Guid parcelId);
    Task<List<ParcelLocation>> GetHistoryByParcelIdAsync(Guid parcelId);
}
