using ValenceHub.Domain.Common.Enums;
using ValenceHub.Domain.Common.ValueObjects;
using ValenceHub.Domain.Events;

namespace ValenceHub.Domain.Users.Events;

public sealed record UserContactUpdatedDomainEvent(
    Guid Id,
    Guid ActionBy,
    string? Email,
    string? PhoneNumber,
    DateTimeOffset OccurredOnUtc
) : DomainEvent(OccurredOnUtc)
{
    public EntityAction Action => EntityAction.Update;
}
