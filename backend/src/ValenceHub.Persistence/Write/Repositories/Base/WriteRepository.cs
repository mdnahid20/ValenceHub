using ValenceHub.Application.Abstractions.Repositories;
using ValenceHub.Persistence.Write.Contexts;

namespace ValenceHub.Persistence.Write.Repositories.Base;

public class WriteRepository<T> : IWriteRepository<T> where T : class
{
    protected readonly ValenceHubWriteDbContext _db;

    public WriteRepository(ValenceHubWriteDbContext db) => _db = db;

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await _db.Set<T>().AddAsync(entity, cancellationToken);

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        => await _db.Set<T>().AddRangeAsync(entities, cancellationToken);

    public void Update(T entity) => _db.Set<T>().Update(entity);

    public void UpdateRange(IEnumerable<T> entities) => _db.Set<T>().UpdateRange(entities);

    public void Remove(T entity) => _db.Set<T>().Remove(entity);

    public void RemoveRange(IEnumerable<T> entities) => _db.Set<T>().RemoveRange(entities);
}
