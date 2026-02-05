using System;
using System.Collections.Generic;
using System.Text;

namespace ValenceHub.Domain.Common.Events
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}
