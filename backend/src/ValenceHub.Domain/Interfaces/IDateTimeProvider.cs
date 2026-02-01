using System;
using System.Collections.Generic;
using System.Text;

namespace ValenceHub.Domain.Interfaces
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }
}
