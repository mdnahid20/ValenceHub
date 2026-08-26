namespace ValenceHub.Application.Abstractions.Events;

public interface IEventPublisher
{
    Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken = default);
}
