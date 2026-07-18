using System;
using System.Collections.Generic;
using System.Text;

namespace ValenceHub.Domain.Events;

public interface IDomainEvent
{
    Guid EventId { get; }
    DateTimeOffset OccurredOnUtc { get; }
}
