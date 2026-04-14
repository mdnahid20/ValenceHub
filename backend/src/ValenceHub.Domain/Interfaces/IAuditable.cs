
using ValenceHub.Domain.Users;

namespace ValenceHub.Domain.Interfaces;

public interface IAuditable
{
    DateTimeOffset CreatedAt { get; }
    DateTimeOffset? UpdatedAt { get; }
    UserId? CreatedBy { get; }
    UserId? UpdatedBy { get; }
}
