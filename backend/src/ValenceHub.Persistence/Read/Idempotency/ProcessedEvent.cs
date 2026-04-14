namespace ValenceHub.Persistence.Read.Idempotency;

public sealed class ProcessedEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid EventId { get; init; }
    public string EventType { get; init; }
    public DateTimeOffset ProcessedOnUtc { get; init; }

    public ProcessedEvent(Guid eventId, string eventType, DateTimeOffset processedOnUtc)
    {
        EventId = eventId;
        EventType = eventType;
        ProcessedOnUtc = processedOnUtc;
    }
}

