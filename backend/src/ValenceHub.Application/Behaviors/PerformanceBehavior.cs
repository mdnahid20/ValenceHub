using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using ValenceHub.Application.Abstractions.Commands;
using ValenceHub.Application.Abstractions.Results;

namespace ValenceHub.Application.Behaviors;

public sealed class PerformanceBehavior<TCommand, TResponse>
    : ICommandBehavior<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    private readonly ILogger<PerformanceBehavior<TCommand, TResponse>> _logger;

    public PerformanceBehavior(
        ILogger<PerformanceBehavior<TCommand, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<Result<TResponse>> Handle(
        TCommand command,
        CommandHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();

        var result = await next(cancellationToken);

        sw.Stop();

        _logger.LogInformation(
            "Command {Command} executed in {Elapsed} ms",
            typeof(TCommand).Name,
            sw.ElapsedMilliseconds);

        return result;
    }
}
