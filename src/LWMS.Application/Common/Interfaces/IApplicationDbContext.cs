using Microsoft.EntityFrameworkCore;
using LWMS.Domain.Entities;

namespace LWMS.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<AuditLog> AuditLogs { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
