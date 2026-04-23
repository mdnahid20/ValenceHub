using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ValenceHub.Application.Abstractions.Commands;
using ValenceHub.Application.Abstractions.Results;

namespace ValenceHub.Application.Behaviors;

public sealed class ExceptionHandlingBehavior<TCommand>
    : ICommandBehavior<TCommand>
    where TCommand : ICommand
{
    private readonly ILogger<ExceptionHandlingBehavior<TCommand>> _logger;

    public ExceptionHandlingBehavior(ILogger<ExceptionHandlingBehavior<TCommand>> logger)
    {
        _logger = logger;
    }

    public async Task<Result> Handle(
        TCommand command,
        CommandHandlerDelegate next,
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

            return Result.Failure(new Error(
                "ExceptionHandling",
                $"An unexpected error occurred while processing {commandName}: {exception.Message}"));
        }
    }
}
