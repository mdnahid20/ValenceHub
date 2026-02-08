using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using ValenceHub.Application.Abstractions.Commands;
using ValenceHub.Application.Abstractions.Results;

namespace ValenceHub.Application.Behaviors;

public sealed class PerformanceBehavior<TCommand>
    : ICommandBehavior<TCommand>
    where TCommand : ICommand
{
    private readonly ILogger<PerformanceBehavior<TCommand>> _logger;

    public PerformanceBehavior(
        ILogger<PerformanceBehavior<TCommand>> logger)
    {
        _logger = logger;
    }

    public async Task<Result> Handle(
        TCommand command,
        CommandHandlerDelegate next,
        CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();

        var result = await next();

        sw.Stop();

        _logger.LogInformation(
            "Command {Command} executed in {Elapsed} ms",
            typeof(TCommand).Name,
            sw.ElapsedMilliseconds);

        return result;
    }
}
