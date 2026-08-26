using ValenceHub.Domain.Common.Primitives;
using ValenceHub.Domain.Events;

namespace ValenceHub.Domain.Abstractions;

public abstract class AggregateRoot<TId, TValue>
    : Entity<TId>, IHasDomainEvents
    where TId : IStronglyTypedId<TValue>
    where TValue : notnull
{
    private readonly List<DomainEvent> _domainEvents = new();

    protected AggregateRoot(TId id) : base(id)
    {
    }
    public override int GetHashCode() => Id?.GetHashCode() ?? 0;

    public IReadOnlyCollection<DomainEvent> GetDomainEvents() => _domainEvents.AsReadOnly();
    public void ClearDomainEvents() => _domainEvents.Clear();

    protected void RaiseDomainEvent(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}
