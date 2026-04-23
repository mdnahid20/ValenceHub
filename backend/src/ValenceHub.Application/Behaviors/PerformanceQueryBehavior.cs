using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ValenceHub.Application.Abstractions.Queries;
using ValenceHub.Application.Abstractions.Results;

namespace ValenceHub.Application.Behaviors;

public sealed class PerformanceQueryBehavior<TQuery, TResponse>
    : IQueryBehavior<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    private readonly ILogger<PerformanceQueryBehavior<TQuery, TResponse>> _logger;

    public PerformanceQueryBehavior(ILogger<PerformanceQueryBehavior<TQuery, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<Result<TResponse>> Handle(
        TQuery query,
        QueryHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();

        var result = await next(cancellationToken);

        sw.Stop();

        _logger.LogInformation(
            "Query {Query} executed in {Elapsed} ms",
            typeof(TQuery).Name,
            sw.ElapsedMilliseconds);

        return result;
    }
}
