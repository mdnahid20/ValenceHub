namespace ValenceHub.Persistence.Read.Idempotency;

public interface IProcessedEventStore
{
    Task<bool> HasProcessedAsync(Guid eventId, CancellationToken cancellationToken = default);

    Task MarkProcessedAsync(
        Guid eventId,
        string eventType,
        DateTime processedOnUtc,
        CancellationToken cancellationToken = default);
}

