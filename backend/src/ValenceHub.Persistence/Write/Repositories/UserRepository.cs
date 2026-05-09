using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ValenceHub.Application.Abstractions.Repositories;
using ValenceHub.Domain.Common.ValueObjects;
using ValenceHub.Domain.Users;
using ValenceHub.Infrastructure.Attributes;
using ValenceHub.Persistence.Write.Contexts;

namespace ValenceHub.Persistence.Write.Repositories;

[AutoRegister(ServiceLifetime.Scoped)]
public class UserRepository : IRepository<User>, IUserRepository
{
    private readonly ValenceHubWriteDbContext _db;

    public UserRepository(ValenceHubWriteDbContext db) => _db = db;

    public IQueryable<User> Query() => _db.Users.AsQueryable();

    public IQueryable<User> QueryIncludingDeleted() => _db.Users.IgnoreQueryFilters().AsQueryable();

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _db.Users.FindAsync(new object[] { id }, cancellationToken) as User;

    public async Task<User?> GetByIdIncludingDeletedAsync(Guid id, CancellationToken cancellationToken = default)
        => await _db.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task<User?> SingleOrDefaultAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
        => await _db.Users.SingleOrDefaultAsync(predicate, cancellationToken);

    public async Task<IReadOnlyList<User>> ListAsync(CancellationToken cancellationToken = default)
        => await _db.Users.ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<User>> ListAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
        => await _db.Users.Where(predicate).ToListAsync(cancellationToken);

    public async Task AddAsync(User entity, CancellationToken cancellationToken = default)
        => await _db.Users.AddAsync(entity, cancellationToken);

    public void Update(User entity) => _db.Users.Update(entity);

    public void Remove(User entity) => _db.Users.Remove(entity);

    public async Task<UserLoginCredentials?> GetCredentialsByEmailAsync(Email email, CancellationToken cancellationToken = default)
        => await _db.Users
            .AsNoTracking()
            .Where(u => u.Email == email)
            .Select(u => new UserLoginCredentials(u.Id.Value, u.PasswordHash))
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<UserLoginCredentials?> GetCredentialsByPhoneNumberAsync(
        PhoneNumber phoneNumber,
        CancellationToken cancellationToken = default)
        => await _db.Users
            .AsNoTracking()
            .Where(u => u.PhoneNumber == phoneNumber)
            .Select(u => new UserLoginCredentials(u.Id.Value, u.PasswordHash))
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
        => await _db.Users.SingleOrDefaultAsync(u => u.Email == email, cancellationToken);

    public async Task<User?> GetByPhoneNumberAsync(PhoneNumber phoneNumber, CancellationToken cancellationToken = default)
        => await _db.Users.SingleOrDefaultAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => await _db.Users.AnyAsync(u => u.Id == id, cancellationToken);

    public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
        => await _db.Users.AnyAsync(u => u.Email == email, cancellationToken);
    public async Task<bool> ExistsByPhoneNumberAsync(PhoneNumber phoneNumber, CancellationToken cancellationToken = default)
        => await _db.Users.AnyAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);
}

