using System;

namespace ValenceHub.Domain.Events;

public abstract record class DomainEvent : IDomainEvent
{
    protected DomainEvent(DateTimeOffset occurredOnUtc)
    {
        OccurredOnUtc = occurredOnUtc;
    }

    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTimeOffset OccurredOnUtc { get; init; }
}
