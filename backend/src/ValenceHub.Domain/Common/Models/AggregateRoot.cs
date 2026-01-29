using System;
using System.Collections.Generic;
using System.Text;
using ValenceHub.Domain.Common.Events;

namespace ValenceHub.Domain.Common.Models;

public abstract class AggregateRoot<T> : Entity<T> 
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;

    protected void AddDomainEvent(IDomainEvent @event)
        => _domainEvents.Add(@event);

    public void ClearDomainEvents()
        => _domainEvents.Clear();
}
