using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ValenceHub.Application.Abstractions.Commands;
using ValenceHub.Application.Abstractions.Results;

namespace ValenceHub.Application.Behaviors;

public sealed class LoggingBehavior<TCommand, TResponse>
    : ICommandBehavior<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    private readonly ILogger<LoggingBehavior<TCommand, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TCommand, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<Result<TResponse>> Handle(
        TCommand command,
        CommandHandlerDelegate<TResponse> next,
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
