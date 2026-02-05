using System;
using System.Collections.Generic;
using System.Text;

namespace ValenceHub.Application.Abstractions.Messaging;

public interface IProjectionHandler<in TEvent>
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken);
}
