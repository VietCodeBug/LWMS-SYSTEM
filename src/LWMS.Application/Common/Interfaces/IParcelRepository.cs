using LWMS.Domain.Entities;
using LWMS.Domain.Enums;

namespace LWMS.Application.Common.Interfaces;
public interface IParcelRepository : IRepository<Parcel>
{
  Task<Parcel?> GetByTrackingCodeAsync(string trackingCode);
  Task<List<Parcel>> GetByStatusAsync(ParcelStatus status);
  Task<List<Parcel>> GetByMerchantAsync(Guid merchantId);
  Task<List<Parcel>> GetSlaAlertAsync(DateTime now);
}