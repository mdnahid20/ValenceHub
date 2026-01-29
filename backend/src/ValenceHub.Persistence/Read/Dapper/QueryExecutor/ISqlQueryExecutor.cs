using Dapper;

namespace ValenceHub.Persistence.Read.Dapper.QueryExecutor;

public interface ISqlQueryExecutor
{
    Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null);
    Task<T?> QuerySingleAsync<T>(string sql, object? parameters = null);
    Task<int> ExecuteAsync(string sql, object? parameters = null);
}
