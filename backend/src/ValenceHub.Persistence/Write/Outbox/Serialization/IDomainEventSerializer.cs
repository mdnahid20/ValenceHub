using System;
using ValenceHub.Domain.Common.Events;

namespace ValenceHub.Persistence.Write.Outbox.Serialization;

public interface IDomainEventSerializer
{
    int Version { get; }

    string Serialize(IDomainEvent domainEvent);

    IDomainEvent Deserialize(string payload, Type eventType);
}

