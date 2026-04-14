using System.Text.Json;
using ValenceHub.Domain.Events;

namespace ValenceHub.Persistence.Write.Outbox.Serialization;

public sealed class DomainEventSerializerV1 : IDomainEventSerializer
{
    public int Version => 1;

    public string Serialize(DomainEvent domainEvent)
        => JsonSerializer.Serialize(domainEvent);

    public DomainEvent Deserialize(string payload, Type eventType)
        => (DomainEvent)JsonSerializer.Deserialize(payload, eventType)!;
}
