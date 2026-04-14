
using ValenceHub.Domain.Users;

namespace ValenceHub.Domain.Interfaces;

public interface ISoftDelete
{
    bool IsDeleted { get; }
    DateTimeOffset? DeletedAt { get; }
    UserId? DeletedBy { get; }
}
