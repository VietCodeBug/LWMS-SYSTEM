using Microsoft.EntityFrameworkCore;
using LWMS.Domain.Entities;

namespace LWMS.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Product> Products { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
