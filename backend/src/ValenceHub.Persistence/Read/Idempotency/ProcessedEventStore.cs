using Dapper;
using ValenceHub.Persistence.Read.Connection;

namespace ValenceHub.Persistence.Read.Idempotency;

public sealed class ProcessedEventStore : IProcessedEventStore
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ProcessedEventStore(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> HasProcessedAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT 1
            FROM ProcessedEvents WITH (NOLOCK)
            WHERE EventId = @EventId
            """;

        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

        var result = await connection.QueryFirstOrDefaultAsync<int?>(
            new CommandDefinition(
                sql,
                new { EventId = eventId },
                cancellationToken: cancellationToken));

        return result.HasValue;
    }

    public async Task MarkProcessedAsync(
        Guid eventId,
        string eventType,
        DateTimeOffset processedOnUtc,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO ProcessedEvents (EventId, EventType, ProcessedOnUtc)
            VALUES (@EventId, @EventType, @ProcessedOnUtc)
            """;

        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    EventId = eventId,
                    EventType = eventType,
                    ProcessedOnUtc = processedOnUtc
                },
                cancellationToken: cancellationToken));
    }
}

