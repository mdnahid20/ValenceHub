using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ValenceHub.Application.Abstractions.Commands;
using ValenceHub.Application.Abstractions.Results;

namespace ValenceHub.Application.Behaviors;

public sealed class ExceptionHandlingBehavior<TCommand, TResponse>
    : ICommandBehavior<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    private readonly ILogger<ExceptionHandlingBehavior<TCommand, TResponse>> _logger;

    public ExceptionHandlingBehavior(ILogger<ExceptionHandlingBehavior<TCommand, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<Result<TResponse>> Handle(
        TCommand command,
        CommandHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next(cancellationToken);
        }
        catch (Exception exception)
        {
            var commandName = typeof(TCommand).Name;

            _logger.LogError(
                exception,
                "Unhandled exception occurred while handling command {Command}: {Message}",
                commandName,
                exception.Message);

            return Result<TResponse>.Failure(new Error(
                "ExceptionHandling",
                $"An unexpected error occurred while processing {commandName}: {exception.Message}"));
        }
    }
}
