using ValenceHub.Domain.Events;

namespace ValenceHub.Persistence.Write.Outbox.Serialization;

public static class DomainEventSerializer
{
    private static readonly IReadOnlyDictionary<int, IDomainEventSerializer> _serializers =
        new Dictionary<int, IDomainEventSerializer>
        {
            { 1, new DomainEventSerializerV1() }
        };

    public static string Serialize(DomainEvent domainEvent, out int version)
    {
        var serializer = _serializers[1];
        version = serializer.Version;
        return serializer.Serialize(domainEvent);
    }

    public static DomainEvent Deserialize(
        string type,
        string payload,
        int version)
    {
        var serializer = _serializers[version];
        var eventType = Type.GetType(type, throwOnError: true)!;

        return serializer.Deserialize(payload, eventType);
    }
}
