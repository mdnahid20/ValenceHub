using System;

namespace ValenceHub.Domain.Primitives
{
    /// <summary>
    /// Lightweight domain event base type.
    /// </summary>
    public abstract class DomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
