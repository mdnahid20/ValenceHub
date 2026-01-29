using System.Data;

namespace ValenceHub.Persistence.Read.Connection;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}

