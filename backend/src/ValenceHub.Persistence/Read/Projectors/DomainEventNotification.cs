using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using ValenceHub.Domain.Common.Events;

namespace ValenceHub.Persistence.Read.Projectors;
public sealed record DomainEventNotification(
    IDomainEvent DomainEvent
) : INotification;