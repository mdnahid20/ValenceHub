using System;
using System.Collections.Generic;
using System.Text;

namespace ValenceHub.Application.Abstractions.Services;  

public interface IDateTimeOffsetProvider
{
    DateTimeOffset UtcNow { get; }
}
