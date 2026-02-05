using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using ValenceHub.Domain.Common.Events;
using ValenceHub.Domain.Common.Models;
using ValenceHub.Persistence.Read.Projectors;

namespace ValenceHub.Persistence.Write.Dispatchers;

public class DomainEventDispatcher
{
    private readonly IMediator _mediator;

    public DomainEventDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task DispatchEventsAsync(DbContext context)
    {
        var aggregates = context.ChangeTracker
            .Entries<AggregateRoot<Guid>>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();

        var events = aggregates.SelectMany(x => x.DomainEvents).ToList();
        aggregates.ForEach(x => x.ClearDomainEvents());

        foreach (var @event in events)
        {
            await DispatchAsync(@event);
        }
    }
    public async Task DispatchAsync(
        IDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var notification = new DomainEventNotification(domainEvent);
        await _mediator.Publish(notification, cancellationToken);
    }
}
