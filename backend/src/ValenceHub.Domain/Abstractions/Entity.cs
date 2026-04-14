using ValenceHub.Domain.Events;

namespace ValenceHub.Domain.Abstractions;

public abstract class Entity<TId>  where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = new();
    protected Entity(TId id)
    {
        Id = id;
    }
    public TId Id { get; }
    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (GetType() != other.GetType())
        {
            return false;
        }

        return EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }
}
