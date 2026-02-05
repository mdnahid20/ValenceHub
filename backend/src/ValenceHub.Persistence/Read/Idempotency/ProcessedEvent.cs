namespace ValenceHub.Persistence.Read.Idempotency;

public sealed class ProcessedEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid EventId { get; init; }
    public string EventType { get; init; }
    public DateTime ProcessedOnUtc { get; init; }

    public ProcessedEvent(Guid eventId, string eventType, DateTime processedOnUtc)
    {
        EventId = eventId;
        EventType = eventType;
        ProcessedOnUtc = processedOnUtc;
    }
}

