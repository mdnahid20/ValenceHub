using ValenceHub.Domain.Common.ValueObjects;
using ValenceHub.Domain.Events;

namespace ValenceHub.Domain.Users.Events;

public sealed record UserCreatedDomainEvent(
    UserId UserId,
    DateTime OccurredOnUtc
) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
}
