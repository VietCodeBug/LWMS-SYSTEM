using LWMS.Domain.Entities;

namespace LWMS.Application.Common.Interfaces;
public interface IUnitOfWork
{
    IParcelRepository Parcels { get; }
    IHubRepository Hubs { get; }
    IMerchantRepository Merchants { get; }
    IUserRepository Users { get; }
    IBagRepository Bags { get; }
    IRepository<ServiceType> ServiceTypes { get; }
    IRepository<TrackingLog> TrackingLogs { get; }
    IRepository<BagItem> BagItems { get; }
    IRepository<ShipperAssignment> ShipperAssignments { get; }
    IRepository<CodRecord> CodRecords { get; }
    IRepository<CodSettlement> CodSettlements { get; }
    IRepository<CodSettlementItem> CodSettlementItems { get; }
    IRepository<ReturnOrder> ReturnOrders { get; }
    IRepository<RefreshToken> RefreshTokens { get; }
    IRackRepository Racks { get; }
    IParcelLocationRepository ParcelLocations { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}