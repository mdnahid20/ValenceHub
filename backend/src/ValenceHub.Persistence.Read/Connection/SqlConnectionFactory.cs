using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using ValenceHub.Infrastructure.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace ValenceHub.Persistence.Read.Connection;

[AutoRegister(ServiceLifetime.Singleton, ServiceType = typeof(IDbConnectionFactory))]
public class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly IConfiguration _configuration;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new SqlConnection(_configuration.GetConnectionString("ReadDb"));
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
