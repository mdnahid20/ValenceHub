namespace ValenceHub.Persistence.Read.Models.Integration;

public sealed class ProcessedEventRecord
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid EventId { get; init; }
    public string EventType { get; init; }
    public DateTimeOffset ProcessedOnUtc { get; init; }

    public ProcessedEventRecord(Guid eventId, string eventType, DateTimeOffset processedOnUtc)
    {
        EventId = eventId;
        EventType = eventType;
        ProcessedOnUtc = processedOnUtc;
    }
}
