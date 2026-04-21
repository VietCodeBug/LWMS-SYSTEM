using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Entities;
using LWMS.Infrastructure.Data;

namespace LWMS.Infrastructure.Repositories;

public class HubRepository : RepositoryBase<Hub>, IHubRepository
{
    public HubRepository(AppDbContext context) : base(context) { }
}

public class MerchantRepository : RepositoryBase<Merchant>, IMerchantRepository
{
    public MerchantRepository(AppDbContext context) : base(context) { }
}

public class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }
}

public class BagRepository : RepositoryBase<Bag>, IBagRepository
{
    public BagRepository(AppDbContext context) : base(context) { }
}
