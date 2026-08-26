using System;

namespace ValenceHub.Domain.Common.Primitives;

public abstract class StronglyTypedId<TValue> :
    IStronglyTypedId<TValue>,
    IEquatable<StronglyTypedId<TValue>>
    where TValue : notnull
{
    public TValue Value { get; }

    protected StronglyTypedId(TValue value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        Value = value;
    }

    public override string ToString() => Value.ToString()!;

    #region Equality

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((StronglyTypedId<TValue>)obj);
    }

    public bool Equals(StronglyTypedId<TValue>? other)
    {
        if (other is null)
        {
            return false;
        }

        return Value.Equals(other.Value);
    }

    public override int GetHashCode()
        => HashCode.Combine(GetType(), Value);

    public static bool operator ==(
        StronglyTypedId<TValue>? left,
        StronglyTypedId<TValue>? right)
        => Equals(left, right);

    public static bool operator !=(
        StronglyTypedId<TValue>? left,
        StronglyTypedId<TValue>? right)
        => !Equals(left, right);

    #endregion
}
