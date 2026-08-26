using ValenceHub.Domain.Events;

namespace ValenceHub.Domain.Users.Events;

public sealed record UserVerifiedDomainEvent(
    Guid Id,
    DateTimeOffset OccurredOnUtc)
    : DomainEvent(OccurredOnUtc);
