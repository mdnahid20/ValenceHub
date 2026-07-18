using ValenceHub.Domain.Common.Enums;
using ValenceHub.Domain.Events;

namespace ValenceHub.Domain.Users.Events;

public sealed record UserDeletedDomainEvent(
    Guid Id,
    Guid ActionBy,
    DateTimeOffset OccurredOnUtc
) : DomainEvent(OccurredOnUtc)
{
    public EntityAction Action => EntityAction.Delete;
}
