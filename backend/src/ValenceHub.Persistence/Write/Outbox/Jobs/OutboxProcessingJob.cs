using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using ValenceHub.Domain.Common.Events;
using ValenceHub.Persistence.Read.Idempotency;
using ValenceHub.Persistence.Read.Projectors;
using ValenceHub.Persistence.Write.Contexts;
using ValenceHub.Persistence.Write.Dispatchers;
using ValenceHub.Persistence.Write.Outbox.Models;
using ValenceHub.Persistence.Write.Outbox.Serialization;

namespace ValenceHub.Persistence.Write.Outbox.Processors;

public sealed class OutboxProcessingJob
{
    private readonly ValenceHubWriteDbContext _context;
    private readonly DomainEventDispatcher _dispatcher;
    private readonly IProcessedEventStore _processedEventStore;

    public OutboxProcessingJob(
        ValenceHubWriteDbContext context,
        DomainEventDispatcher dispatcher,
        IProcessedEventStore processedEventStore)
    {
        _context = context;
        _dispatcher = dispatcher;
        _processedEventStore = processedEventStore;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var messages = await _context.Set<OutboxMessage>()
            .Where(x => x.ProcessedOnUtc == null)
            .OrderBy(x => x.OccurredOnUtc)
            .Take(20)
            .ToListAsync(cancellationToken);

        if (!messages.Any()) return;

        foreach (var message in messages)
        {
            try
            {
                // Idempotency check against the read database
                if (await _processedEventStore.HasProcessedAsync(message.Id, cancellationToken))
                {
                    message.MarkProcessed();
                    continue;
                }

                var domainEvent = DomainEventSerializer.Deserialize(
                    message.Type,
                    message.Payload,
                    message.Version);

                await _dispatcher.DispatchAsync(domainEvent, cancellationToken);

                await _processedEventStore.MarkProcessedAsync(
                    message.Id,
                    message.Type,
                    DateTime.UtcNow,
                    cancellationToken);

                message.MarkProcessed();
            }
            catch (Exception ex)
            {
                message.MarkFailed(ex.Message);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}