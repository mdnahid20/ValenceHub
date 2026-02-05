using System;
namespace ValenceHub.Domain.Common.Events;

public abstract class DomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
