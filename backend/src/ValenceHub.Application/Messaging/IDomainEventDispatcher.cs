using ValenceHub.Domain.Events;

namespace ValenceHub.Application.Messaging;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(
        IEnumerable<DomainEvent> domainEvents,
        CancellationToken cancellationToken);
}
