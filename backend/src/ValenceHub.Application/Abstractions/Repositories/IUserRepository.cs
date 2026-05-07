using System.Threading;
using ValenceHub.Domain.Common.ValueObjects;
using ValenceHub.Domain.Users;

namespace ValenceHub.Application.Abstractions.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<User?> GetByPhoneNumberAsync(PhoneNumber phoneNumber, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByPhoneNumberAsync(PhoneNumber phoneNumber, CancellationToken cancellationToken = default);
}
