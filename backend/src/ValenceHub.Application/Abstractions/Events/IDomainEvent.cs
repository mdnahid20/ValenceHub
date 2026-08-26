namespace ValenceHub.Application.Abstractions.Events;

public interface IDomainEvent
{
    DateTimeOffset OccurredOn { get; }
}
