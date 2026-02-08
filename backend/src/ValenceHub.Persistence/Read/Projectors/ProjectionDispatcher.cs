using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using ValenceHub.Application.Messaging;
using ValenceHub.Domain.Common.Events;

namespace ValenceHub.Persistence.Read.Projectors;

public sealed class ProjectionDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public ProjectionDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync(
        IDomainEvent domainEvent,
        CancellationToken cancellationToken)
    {
        var handlerType = typeof(IProjectionHandler<>)
            .MakeGenericType(domainEvent.GetType());

        var handlers = _serviceProvider.GetServices(handlerType);

        foreach (dynamic handler in handlers)
        {
            await handler.HandleAsync((dynamic)domainEvent, cancellationToken);
        }
    }
}
