using Microsoft.EntityFrameworkCore.Storage;
using ValenceHub.Application.Abstractions.Transactions;
using ValenceHub.Domain.Abstractions;
using ValenceHub.Domain.Events;
using ValenceHub.Persistence.Write.Contexts;
using ValenceHub.Persistence.Write.Outbox.Models;
using ValenceHub.Persistence.Write.Outbox.Serialization;

namespace ValenceHub.Persistence.Write.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ValenceHubWriteDbContext _context;
    private IDbContextTransaction? _currentTransaction;

    public UnitOfWork(ValenceHubWriteDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = ExtractDomainEvents();
        var outboxMessages = domainEvents
            .Select(ToOutboxMessage)
            .ToList();

        if (outboxMessages.Any())
        {
            await _context.Set<OutboxMessage>().AddRangeAsync(outboxMessages, cancellationToken);
        }

        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null) return;
        _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null) return;

        await _currentTransaction.CommitAsync(cancellationToken);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null) return;
        await _currentTransaction.RollbackAsync(cancellationToken);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }

        await _context.DisposeAsync();
    }

    public IReadOnlyCollection<dynamic> GetAggregateRoots()
    {
        return _context.ChangeTracker
            .Entries<IHasDomainEvents>()
            .Select(entry => entry.Entity)
            .Cast<dynamic>()
            .ToList()!;
    }

    private IReadOnlyList<DomainEvent> ExtractDomainEvents()
    {
        var domainEvents = _context.ChangeTracker
        .Entries<IHasDomainEvents>()
        .Select(entry => entry.Entity)
        .SelectMany(entity =>
        {
            var events = entity.GetDomainEvents().ToList();
            if (events.Any())
            {
                entity.ClearDomainEvents();
            }

            return events;
        })
        .ToList();

        return domainEvents;
    }
    private static OutboxMessage ToOutboxMessage(DomainEvent domainEvent)
    {
        var payload = DomainEventSerializer.Serialize(domainEvent, out var version);

        return new OutboxMessage(
            type: domainEvent.GetType().AssemblyQualifiedName!,
            payload: payload,
            version: version,
            occurredOnUtc: domainEvent.OccurredOnUtc);
    }
}
