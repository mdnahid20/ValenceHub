using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ValenceHub.Application.Common.Clock;
using ValenceHub.Domain.Common.Enums;
using ValenceHub.Persistence.Read.Models.Users;

namespace ValenceHub.Persistence.Read.Helpers.Audit;

public sealed class AuditInterceptor : SaveChangesInterceptor
{
    private readonly IClock _clock;

    public AuditInterceptor(IClock clock)
    {
        _clock = clock;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var audits = new List<AuditLog>();

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is AuditLog)
            {
                continue;
            }

            var entityType = entry.Metadata.ClrType;
            if (!Attribute.IsDefined(entityType, typeof(AuditableEntityAttribute), inherit: true))
            {
                continue;
            }

            if (entry.State is not (EntityState.Added or EntityState.Modified or EntityState.Deleted))
            {
                continue;
            }

            object? oldValues = null;
            object? newValues = null;

            if (entry.State == EntityState.Modified)
            {
                oldValues = entry.OriginalValues.ToObject();
                newValues = entry.CurrentValues.ToObject();
            }
            else if (entry.State == EntityState.Added)
            {
                newValues = entry.CurrentValues.ToObject();
            }
            else if (entry.State == EntityState.Deleted)
            {
                oldValues = entry.OriginalValues.ToObject();
            }

            var changes = entry.State switch
            {
                EntityState.Added when newValues != null => ChangedDataBuilder.BuildDiff(null, newValues),
                EntityState.Modified when newValues != null => ChangedDataBuilder.BuildDiff(oldValues, newValues),
                EntityState.Deleted when oldValues != null => ChangedDataBuilder.BuildDiff(null, oldValues),
                _ => new Dictionary<string, object?>()
            };

            var action = MapAction(entry.State);
            if (action == EntityAction.Update && changes.Count == 0)
            {
                continue;
            }

            var entityId = GetGuidProperty(entry, "Id");
            var eventId = GetGuidProperty(entry, "EventId");

            audits.Add(new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = entityId,
                EntityId = entityId,
                EntityName = entityType.Name,
                Action = action,
                ActionBy = Guid.Empty,
                EventId = eventId,
                OccurredOnUtc = _clock.UtcNow,
                ChangedData = changes
            });
        }

        if (audits.Count > 0)
        {
            context.Set<AuditLog>().AddRange(audits);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static EntityAction MapAction(EntityState state) => state switch
    {
        EntityState.Added => EntityAction.Create,
        EntityState.Modified => EntityAction.Update,
        EntityState.Deleted => EntityAction.Delete,
        _ => throw new ArgumentOutOfRangeException(nameof(state), state, "Unsupported entity state for auditing.")
    };

    private static Guid GetGuidProperty(EntityEntry entry, string propertyName)
    {
        var property = entry.Properties.FirstOrDefault(p => p.Metadata.Name == propertyName);

        return property?.CurrentValue switch
        {
            Guid value => value,
            _ => Guid.Empty
        };
    }
}
