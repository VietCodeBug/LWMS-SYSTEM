using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using LWMS.Domain.Entities;
using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Common;
using System.Text.Json;

namespace LWMS.Infrastructure.Data.Interceptors;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;

    public AuditInterceptor(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateAuditLogs(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateAuditLogs(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditLogs(DbContext? context)
    {
        if (context == null) return;

        var entries = context.ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
            .ToList();

        if (!entries.Any()) return;

        var userId = _currentUserService.UserId != null ? Guid.Parse(_currentUserService.UserId) : Guid.Empty;
        var role = _currentUserService.Role ?? "System";

        foreach (var entry in entries)
        {
            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                ActorId = userId,
                ActorRole = role,
                Action = entry.State.ToString().ToUpper(),
                EntityType = entry.Entity.GetType().Name,
                EntityId = entry.Entity.Id,
                CreatedTime = DateTime.UtcNow,
                Changes = GetChanges(entry)
            };

            context.Set<AuditLog>().Add(auditLog);
        }
    }

    private string GetChanges(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
    {
        var changes = new Dictionary<string, object>();

        if (entry.State == EntityState.Modified)
        {
            foreach (var property in entry.Properties)
            {
                if (property.IsModified)
                {
                    var oldVal = property.OriginalValue;
                    var newVal = property.CurrentValue;

                    // Nếu là Enum, chuyển sang string cho dễ đọc
                    if (oldVal != null && oldVal.GetType().IsEnum) oldVal = oldVal.ToString();
                    if (newVal != null && newVal.GetType().IsEnum) newVal = newVal.ToString();

                    changes[property.Metadata.Name] = new
                    {
                        Old = oldVal,
                        New = newVal
                    };
                }
            }
        }
        else if (entry.State == EntityState.Added)
        {
            foreach (var property in entry.Properties)
            {
                var val = property.CurrentValue;
                if (val != null && val.GetType().IsEnum) val = val.ToString();
                changes[property.Metadata.Name] = val!;
            }
        }

        return JsonSerializer.Serialize(changes);
    }
}
