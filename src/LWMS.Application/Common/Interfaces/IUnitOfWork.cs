namespace LWMS.Application.Common.Interfaces;
public interface IUnitOfWork
{
    IParcelRepository Parcels { get; }
    IHubRepository Hubs { get; }
    IMerchantRepository Merchants { get; }
    IUserRepository Users { get; }
    IBagRepository Bags { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}