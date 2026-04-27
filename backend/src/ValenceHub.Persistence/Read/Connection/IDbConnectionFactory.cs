using Microsoft.Extensions.DependencyInjection;
using System.Data;
using ValenceHub.Infrastructure.Attributes;

namespace ValenceHub.Persistence.Read.Connection;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}

