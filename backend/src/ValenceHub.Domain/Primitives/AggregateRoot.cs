using System.Collections.Generic;

namespace ValenceHub.Domain.Primitives
{
    /// <summary>
    /// Aggregate root that collects domain events.
    /// </summary>
    public abstract class AggregateRoot : Entity
    {
        private readonly List<DomainEvent> _events = new();

        protected AggregateRoot() : base() { }
        protected AggregateRoot(System.Guid id) : base(id) { }

        public IReadOnlyCollection<DomainEvent> Events => _events.AsReadOnly();

        protected void AddEvent(DomainEvent @event) => _events.Add(@event);
        public void ClearEvents() => _events.Clear();
    }
}
