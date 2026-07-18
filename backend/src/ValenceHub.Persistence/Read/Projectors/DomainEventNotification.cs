using MediatR;
using ValenceHub.Domain.Events;

namespace ValenceHub.Persistence.Read.Projectors;

public sealed record DomainEventNotification<TDomainEvent>(
    TDomainEvent DomainEvent
) : INotification
    where TDomainEvent : DomainEvent;
