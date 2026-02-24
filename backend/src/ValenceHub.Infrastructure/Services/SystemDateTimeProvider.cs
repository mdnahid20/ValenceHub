using System;
using System.Collections.Generic;
using System.Text;
using ValenceHub.Application.Abstractions.Services;

namespace ValenceHub.Infrastructure.Services
{
    public class SystemDateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
