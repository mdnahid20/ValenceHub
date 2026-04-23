using System;
using Microsoft.Extensions.DependencyInjection;
using ValenceHub.Application.Abstractions.Services;
using ValenceHub.Infrastructure.Attributes;

namespace ValenceHub.Infrastructure.Services
{
    [AutoRegister(ServiceLifetime.Singleton)]
    public class SystemDateTimeOffsetProvider : IDateTimeOffsetProvider
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}

