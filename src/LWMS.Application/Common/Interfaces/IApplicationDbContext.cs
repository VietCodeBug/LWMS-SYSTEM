using Microsoft.EntityFrameworkCore;
using LWMS.Domain.Entities;

namespace LWMS.Application.Common.Interfaces
{
public interface IApplicationDbContext
{
    DbSet<Parcel> Parcels { get; }
    DbSet<Bag> Bags { get; }
    DbSet<Hub> Hubs { get; }
    DbSet<Merchant> Merchants { get; }
    DbSet<User> Users { get; }
    DbSet<CodRecord> CodRecords { get; }
    DbSet<ShipperAssignment> ShipperAssignments { get; }
    DbSet<TrackingLog> TrackingLogs { get; }
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<ServiceType> ServiceTypes { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
}
