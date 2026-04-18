using System;
using Microsoft.Extensions.DependencyInjection;
using ValenceHub.Application.Abstractions.Services;
using ValenceHub.Application.Common.Clock;
using ValenceHub.Infrastructure.Attributes;

namespace ValenceHub.Infrastructure.Services
{
    [AutoRegister(ServiceLifetime.Singleton)]
    public class SystemDateTimeOffsetProvider : IDateTimeOffsetProvider, IClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
