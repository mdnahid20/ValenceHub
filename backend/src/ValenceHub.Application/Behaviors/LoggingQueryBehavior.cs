using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ValenceHub.Application.Abstractions.Queries;
using ValenceHub.Application.Abstractions.Results;

namespace ValenceHub.Application.Behaviors;

public sealed class LoggingQueryBehavior<TQuery, TResponse>
    : IQueryBehavior<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    private readonly ILogger<LoggingQueryBehavior<TQuery, TResponse>> _logger;

    public LoggingQueryBehavior(ILogger<LoggingQueryBehavior<TQuery, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<Result<TResponse>> Handle(
        TQuery query,
        QueryHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var name = typeof(TQuery).Name;

        _logger.LogInformation("Handling query {Query}", name);

        var result = await next(cancellationToken);

        if (result.IsFailure)
        {
            _logger.LogWarning(
                "Query {Query} failed: {Error}",
                name,
                result.Error?.Message);
        }

        return result;
    }
}
