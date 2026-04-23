using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ValenceHub.Application.Abstractions.Commands;
using ValenceHub.Application.Abstractions.Results;

namespace ValenceHub.Application.Behaviors;

public sealed class LoggingBehavior<TCommand>
    : ICommandBehavior<TCommand>
    where TCommand : ICommand
{
    private readonly ILogger<LoggingBehavior<TCommand>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TCommand>> logger)
    {
        _logger = logger;
    }

    public async Task<Result> Handle(
        TCommand command,
        CommandHandlerDelegate next,
        CancellationToken cancellationToken)
    {
        var name = typeof(TCommand).Name;

        _logger.LogInformation("Handling command {Command}", name);

        var result = await next(cancellationToken);

        if (result.IsFailure)
        {
            _logger.LogWarning(
                "Command {Command} failed: {Error}",
                name,
                result.Error?.Message);
        }

        return result;
    }
}
