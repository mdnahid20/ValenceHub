namespace ValenceHub.Domain.Common.Primitives;

public interface IStronglyTypedId<TValue>
    where TValue : notnull
{
    TValue Value { get; }
}
