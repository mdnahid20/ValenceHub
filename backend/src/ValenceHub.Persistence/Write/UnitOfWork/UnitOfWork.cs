using Microsoft.EntityFrameworkCore.Storage;
using ValenceHub.Application.Abstractions.Repositories;
using ValenceHub.Persistence.Write.Contexts;
using ValenceHub.Persistence.Write.Dispatchers;

namespace ValenceHub.Persistence.Write.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ValenceHubWriteDbContext _context;
    private IDbContextTransaction? _currentTransaction;
    private readonly DomainEventDispatcher _dispatcher;

    public UnitOfWork(ValenceHubWriteDbContext context, DomainEventDispatcher dispatcher)
    {
        _context = context;
        _dispatcher = dispatcher;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await _context.SaveChangesAsync(cancellationToken);
        await _dispatcher.DispatchEventsAsync(_context);
        return result;
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null) return;
        _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null) return;
        await _context.SaveChangesAsync(cancellationToken);
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
}