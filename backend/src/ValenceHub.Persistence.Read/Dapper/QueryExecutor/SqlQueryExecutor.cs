using Dapper;
using ValenceHub.Persistence.Read.Connection;
using ValenceHub.Infrastructure.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace ValenceHub.Persistence.Read.Dapper.QueryExecutor;

[AutoRegister(ServiceLifetime.Scoped, ServiceType = typeof(ISqlQueryExecutor))]
public class SqlQueryExecutor : ISqlQueryExecutor
{
    private readonly IDbConnectionFactory _connectionFactory;

    public SqlQueryExecutor(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QueryAsync<T>(sql, parameters);
    }

    public async Task<T?> QuerySingleAsync<T>(string sql, object? parameters = null)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QuerySingleOrDefaultAsync<T>(sql, parameters);
    }

    public async Task<int> ExecuteAsync(string sql, object? parameters = null)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.ExecuteAsync(sql, parameters);
    }
}
