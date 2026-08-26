using ValenceHub.Domain.Common.Primitives;

namespace ValenceHub.Domain.Users;

public sealed class UserId : StronglyTypedId<Guid>
{
    private UserId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(value));
    }

    public static UserId New() => new(Guid.NewGuid());

    public static UserId FromGuid(Guid value) => new(value);

    public static implicit operator Guid(UserId userId) => userId.Value;
}
