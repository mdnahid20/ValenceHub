using System;
using System.Collections.Generic;
using System.Text;
using ValenceHub.Domain.Interfaces;

namespace ValenceHub.Infrastructure.Services
{
    public class SystemDateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
