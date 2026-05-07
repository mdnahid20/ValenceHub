using ValenceHub.Domain.Common.Enums;
using ValenceHub.Domain.Events;

namespace ValenceHub.Domain.Users.Events;

public sealed record UserCreatedDomainEvent(
    Guid Id,
    Guid ActionBy,
    string? Email,
    string? PhoneNumber,
    DateTimeOffset OccurredOnUtc
)
    : DomainEvent(OccurredOnUtc)
{
    public EntityAction Action => EntityAction.Create;
}