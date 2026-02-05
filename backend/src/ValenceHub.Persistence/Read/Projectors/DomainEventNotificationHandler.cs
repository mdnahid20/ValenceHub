using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using ValenceHub.Domain.Common.Events;

namespace ValenceHub.Persistence.Read.Projectors;

public sealed class DomainEventNotificationHandler
    : INotificationHandler<DomainEventNotification>
{
    private readonly ProjectionDispatcher _dispatcher;

    public DomainEventNotificationHandler(ProjectionDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public async Task Handle(
        DomainEventNotification notification,
        CancellationToken cancellationToken)
    {
        await _dispatcher.DispatchAsync(
            notification.DomainEvent,
            cancellationToken);
    }
}