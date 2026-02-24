using System;

namespace ValenceHub.Domain.Events;

public abstract class DomainEvent : IDomainEvent
{
    protected DomainEvent(DateTime occurredOnUtc)
    {
        EventId = Guid.NewGuid();
        OccurredOnUtc = occurredOnUtc;
    }
    public Guid EventId { get; }
    public DateTime OccurredOnUtc { get; }
}
