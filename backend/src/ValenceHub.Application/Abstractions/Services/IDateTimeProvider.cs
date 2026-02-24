using System;
using System.Collections.Generic;
using System.Text;

namespace ValenceHub.Application.Abstractions.Services;  

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
