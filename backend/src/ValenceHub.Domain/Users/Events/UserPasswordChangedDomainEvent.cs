using ValenceHub.Domain.Events;

namespace ValenceHub.Domain.Users.Events;

public sealed record UserPasswordChangedDomainEvent(
    Guid UserId,
    DateTimeOffset OccurredOnUtc
) : DomainEvent(OccurredOnUtc);