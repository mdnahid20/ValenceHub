using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ValenceHub.Infrastructure.Attributes;
using ValenceHub.Persistence.Read.Contexts;
using ValenceHub.Persistence.Read.Models.Users;

namespace ValenceHub.Persistence.Read.Repositories.Users;

[AutoRegister(ServiceLifetime.Scoped)]
public sealed class UserReadRepository : IUserReadRepository
{
    private readonly ReadDbContext _db;

    public UserReadRepository(ReadDbContext db) => _db = db;

    public async Task<UserReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task<UserReadModel?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email != null && u.Email == email, cancellationToken);

    public async Task<UserReadModel?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
        => await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.PhoneNumber != null && u.PhoneNumber == phoneNumber, cancellationToken);

    public async Task<IReadOnlyList<UserReadModel>> ListAsync(CancellationToken cancellationToken = default)
        => await _db.Users.AsNoTracking().ToListAsync(cancellationToken);
}
