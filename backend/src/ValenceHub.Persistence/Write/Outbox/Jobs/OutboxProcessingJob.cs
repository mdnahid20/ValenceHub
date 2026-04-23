using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ValenceHub.Application.Abstractions.Services;
using ValenceHub.Application.Messaging;
using ValenceHub.Infrastructure.Attributes;
using ValenceHub.Persistence.Read.Idempotency;
using ValenceHub.Persistence.Write.Contexts;
using ValenceHub.Persistence.Write.Outbox.Models;
using ValenceHub.Persistence.Write.Outbox.Serialization;

namespace ValenceHub.Persistence.Write.Outbox.Processors;

[AutoRegister(ServiceLifetime.Scoped)]
public sealed class OutboxProcessingJob
{
    private readonly ValenceHubWriteDbContext _context;
    private readonly IDateTimeOffsetProvider _dateTimeProvider;
    private readonly IDomainEventDispatcher _dispatcher;
    private readonly IProcessedEventStore _processedEventStore;

    public OutboxProcessingJob(
        ValenceHubWriteDbContext context,
        IDateTimeOffsetProvider dateTimeProvider,
        IDomainEventDispatcher dispatcher,
        IProcessedEventStore processedEventStore)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
        _dispatcher = dispatcher;
        _processedEventStore = processedEventStore;
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
                    message.MarkProcessed(_dateTimeProvider.UtcNow);
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
                    _dateTimeProvider.UtcNow,
                    cancellationToken);

                message.MarkProcessed(_dateTimeProvider.UtcNow);
            }
            catch (Exception ex)
            {
                message.MarkFailed(ex.Message);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}

