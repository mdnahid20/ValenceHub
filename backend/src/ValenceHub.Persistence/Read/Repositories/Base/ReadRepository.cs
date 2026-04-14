using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ValenceHub.Application.Abstractions.Repositories;
using ValenceHub.Persistence.Read.Contexts;

namespace ValenceHub.Persistence.Read.Repositories.Base;

public class ReadRepository<T> : IReadRepository<T> where T : class
{
    protected readonly ReadDbContext _db;

    public ReadRepository(ReadDbContext db) => _db = db;

    public async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
        => await _db.Set<T>()
            .AsNoTracking()
            .FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<IReadOnlyList<T>> ListAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IQueryable<T>>? queryOptions = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _db.Set<T>().AsNoTracking();

        if (predicate is not null)
            query = query.Where(predicate);

        if (queryOptions is not null)
            query = queryOptions(query);

        return await query.ToListAsync(cancellationToken);
    }

    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => _db.Set<T>().AnyAsync(predicate, cancellationToken);

    public Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
        => predicate is null
            ? _db.Set<T>().CountAsync(cancellationToken)
            : _db.Set<T>().CountAsync(predicate, cancellationToken);
}
