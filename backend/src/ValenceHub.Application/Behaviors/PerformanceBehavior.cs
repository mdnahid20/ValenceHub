using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ValenceHub.Application.Behaviors;

public class PerformanceBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly Stopwatch _timer = new();

    public PerformanceBehavior(
        ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsed = _timer.ElapsedMilliseconds;
        if (elapsed > 500)
        {
            _logger.LogWarning(
                "Long Running Request: {RequestName} ({Elapsed}ms)",
                typeof(TRequest).Name, elapsed);
        }

        return response;
    }
}
