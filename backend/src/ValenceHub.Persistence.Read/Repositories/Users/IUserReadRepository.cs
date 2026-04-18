using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ValenceHub.Persistence.Read.Models.Users;

namespace ValenceHub.Persistence.Read.Repositories.Users;

public interface IUserReadRepository
{
    Task<UserReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserReadModel?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserReadModel?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserReadModel>> ListAsync(CancellationToken cancellationToken = default);
}
