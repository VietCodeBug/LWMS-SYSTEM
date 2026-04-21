using LWMS.Application.Common.Interfaces;
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

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IParcelRepository Parcels => _parcels ??= new ParcelRepository(_context);
    public IHubRepository Hubs => _hubs ??= new HubRepository(_context);
    public IMerchantRepository Merchants => _merchants ??= new MerchantRepository(_context);
    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IBagRepository Bags => _bags ??= new BagRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}