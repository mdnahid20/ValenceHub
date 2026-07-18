using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ValenceHub.Application.Messaging;
using ValenceHub.Domain.Events;
using ValenceHub.Infrastructure.Attributes;
using ValenceHub.Persistence.Read.Projectors;

namespace ValenceHub.Persistence.Write.Dispatchers;

[AutoRegister(ServiceLifetime.Scoped)]
public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _mediator;

    public DomainEventDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task DispatchAsync(
        IEnumerable<DomainEvent> domainEvents,
        CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            var notificationType = typeof(DomainEventNotification<>)
                .MakeGenericType(domainEvent.GetType());

            var notification = Activator.CreateInstance(
                notificationType,
                domainEvent);

            await _mediator.Publish((INotification)notification!, cancellationToken);
        }
    }
}
