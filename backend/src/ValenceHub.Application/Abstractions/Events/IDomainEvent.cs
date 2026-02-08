namespace ValenceHub.Application.Abstractions.Events;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
