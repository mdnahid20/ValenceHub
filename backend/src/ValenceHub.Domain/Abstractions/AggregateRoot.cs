using ValenceHub.Domain.Common.Primitives;
using ValenceHub.Domain.Events;

namespace ValenceHub.Domain.Abstractions;

public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : IStronglyTypedId<Guid>
{
    private readonly List<IDomainEvent> _domainEvents = new();

    protected AggregateRoot(TId id) : base(id)
    {
    }
}
