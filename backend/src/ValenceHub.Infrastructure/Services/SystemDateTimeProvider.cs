using System;
using ValenceHub.Application.Abstractions.Services;
using ValenceHub.Application.Common.Clock;

namespace ValenceHub.Infrastructure.Services
{
    public class SystemDateTimeOffsetProvider : IDateTimeOffsetProvider, IClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
