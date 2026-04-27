using System.Threading;
using ValenceHub.Domain.Users;

namespace ValenceHub.Application.Abstractions.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByPhoneAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
}
