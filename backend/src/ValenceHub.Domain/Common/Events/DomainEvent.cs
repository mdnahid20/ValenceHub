using System;
using System.Collections.Generic;
using System.Text;

namespace ValenceHub.Domain.Common.Events
{
    public abstract class DomainEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; protected set; } = DateTime.UtcNow;
    }
}
