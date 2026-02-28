using ValenceHub.Domain.Events;

namespace ValenceHub.Domain.Users.Events;

public sealed record UserDeletedDomainEvent(
    UserId UserId,
    DateTime OccurredOnUtc
) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
}
