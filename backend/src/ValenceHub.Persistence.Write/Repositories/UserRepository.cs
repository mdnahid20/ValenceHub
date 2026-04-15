using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ValenceHub.Application.Abstractions.Repositories;
using ValenceHub.Domain.Users;
using ValenceHub.Persistence.Write.Contexts;

namespace ValenceHub.Persistence.Write.Repositories;

public class UserRepository : IRepository<User>, IUserRepository
{
    private readonly ValenceHubWriteDbContext _db;

    public UserRepository(ValenceHubWriteDbContext db) => _db = db;

    public IQueryable<User> Query() => _db.Users.AsQueryable();

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _db.Users.FindAsync(new object[] { id }, cancellationToken) as User;

    public async Task<User?> SingleOrDefaultAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
        => await _db.Users.SingleOrDefaultAsync(predicate, cancellationToken);

    public async Task<IReadOnlyList<User>> ListAsync(CancellationToken cancellationToken = default)
        => await _db.Users.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<User>> ListAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
        => await _db.Users.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public async Task AddAsync(User entity, CancellationToken cancellationToken = default)
        => await _db.Users.AddAsync(entity, cancellationToken);

    public void Update(User entity) => _db.Users.Update(entity);

    public void Remove(User entity) => _db.Users.Remove(entity);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _db.Users.SingleOrDefaultAsync(u => u.Email != null && u.Email.Value == email, cancellationToken);

    public async Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
        => await _db.Users.SingleOrDefaultAsync(u => u.PhoneNumber != null && u.PhoneNumber.Value == phoneNumber, cancellationToken);
}
