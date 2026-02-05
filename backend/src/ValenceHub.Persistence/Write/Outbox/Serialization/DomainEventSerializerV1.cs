using System;
using System.Text.Json;
using ValenceHub.Domain.Common.Events;

namespace ValenceHub.Persistence.Write.Outbox.Serialization;

public sealed class DomainEventSerializerV1 : IDomainEventSerializer
{
    public int Version => 1;

    public string Serialize(IDomainEvent domainEvent)
        => JsonSerializer.Serialize(domainEvent);

    public IDomainEvent Deserialize(string payload, Type eventType)
        => (IDomainEvent)JsonSerializer.Deserialize(payload, eventType)!;
}

