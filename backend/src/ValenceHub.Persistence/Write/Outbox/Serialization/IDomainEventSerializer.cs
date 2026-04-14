using ValenceHub.Domain.Events;

namespace ValenceHub.Persistence.Write.Outbox.Serialization;

public interface IDomainEventSerializer
{
    int Version { get; }

    string Serialize(DomainEvent domainEvent);
    DomainEvent Deserialize(string payload, Type eventType);
}
