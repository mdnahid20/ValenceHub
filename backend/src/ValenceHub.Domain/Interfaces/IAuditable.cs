
using ValenceHub.Domain.Users;

namespace ValenceHub.Domain.Interfaces;

public interface IAuditable
{
    DateTime CreatedAt { get; }
    DateTime? UpdatedAt { get; }
    UserId? CreatedBy { get; }
    UserId? UpdatedBy { get; }
}
