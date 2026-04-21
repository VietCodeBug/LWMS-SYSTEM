using LWMS.Domain.Entities;
namespace LWMS.Application.Common.Interfaces;

public interface IHubRepository : IRepository<Hub> { }
public interface IMerchantRepository : IRepository<Merchant> { }
public interface IUserRepository : IRepository<User> { }
public interface IBagRepository : IRepository<Bag> { }
