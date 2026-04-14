using Microsoft.EntityFrameworkCore;
using ValenceHub.Application.Common.Clock;
using ValenceHub.Application.Messaging;
using ValenceHub.Persistence.Read.Idempotency;
using ValenceHub.Persistence.Write.Contexts;
using ValenceHub.Persistence.Write.Outbox.Models;
using ValenceHub.Persistence.Write.Outbox.Serialization;

namespace ValenceHub.Persistence.Write.Outbox.Processors;

public sealed class OutboxProcessingJob
{
    private readonly ValenceHubWriteDbContext _context;
    private readonly IDomainEventDispatcher _dispatcher;
    private readonly IProcessedEventStore _processedEventStore;
    private readonly IClock _clock;

    public OutboxProcessingJob(
        ValenceHubWriteDbContext context,
        IDomainEventDispatcher dispatcher,
        IProcessedEventStore processedEventStore,
        IClock clock)
    {
        _context = context;
        _dispatcher = dispatcher;
        _processedEventStore = processedEventStore;
        _clock = clock;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var messages = await _context.Set<OutboxMessage>()
            .Where(x => x.ProcessedOnUtc == null)
            .OrderBy(x => x.OccurredOnUtc)
            .Take(50)
            .ToListAsync(cancellationToken);

        if (!messages.Any())
        {
            return;
        }

        foreach (var message in messages)
        {
            try
            {
                if (await _processedEventStore.HasProcessedAsync(message.Id, cancellationToken))
                {
                    message.MarkProcessed(_clock.UtcNow);
                    continue;
                }

                var domainEvent = DomainEventSerializer.Deserialize(
                    message.Type,
                    message.Payload,
                    message.Version);

                await _dispatcher.DispatchAsync(new[] { domainEvent }, cancellationToken);

                await _processedEventStore.MarkProcessedAsync(
                    message.Id,
                    message.Type,
                    _clock.UtcNow,
                    cancellationToken);

                message.MarkProcessed(_clock.UtcNow);
            }
            catch (Exception ex)
            {
                message.MarkFailed(ex.Message);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
