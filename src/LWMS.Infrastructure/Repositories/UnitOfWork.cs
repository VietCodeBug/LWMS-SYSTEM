using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Entities;
using LWMS.Infrastructure.Data;

namespace LWMS.Infrastructure.Repositories;
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    
    private IParcelRepository? _parcels;
    private IHubRepository? _hubs;
    private IMerchantRepository? _merchants;
    private IUserRepository? _users;
    private IBagRepository? _bags;
    private IRepository<TrackingLog>? _trackingLogs;
    private IRepository<BagItem>? _bagItems;
    private IRepository<CodRecord>? _codRecords;
    private IRepository<ShipperAssignment>? _shipperAssignments;
    private IRepository<ReturnOrder>? _returnOrders;
    private IRepository<ServiceType>? _serviceTypes;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IParcelRepository Parcels => _parcels ??= new ParcelRepository(_context);
    public IHubRepository Hubs => _hubs ??= new HubRepository(_context);
    public IMerchantRepository Merchants => _merchants ??= new MerchantRepository(_context);
    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IBagRepository Bags => _bags ??= new BagRepository(_context);
    public IRepository<TrackingLog> TrackingLogs => _trackingLogs ??= new RepositoryBase<TrackingLog>(_context);
    public IRepository<BagItem> BagItems => _bagItems ??= new RepositoryBase<BagItem>(_context);
    public IRepository<CodRecord> CodRecords => _codRecords ??= new RepositoryBase<CodRecord>(_context);
    public IRepository<ShipperAssignment> ShipperAssignments => _shipperAssignments ??= new RepositoryBase<ShipperAssignment>(_context);
    public IRepository<ReturnOrder> ReturnOrders => _returnOrders ??= new RepositoryBase<ReturnOrder>(_context);
    public IRepository<ServiceType> ServiceTypes => _serviceTypes ??= new RepositoryBase<ServiceType>(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}