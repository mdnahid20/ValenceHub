using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using ValenceHub.Application.Abstractions.Commands;
using ValenceHub.Application.Abstractions.Results;
using ValenceHub.Application.Behaviors;

namespace ValenceHub.Application.Messaging;

internal sealed class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public CommandDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<Result<TResponse>> Dispatch<TResponse>(
        ICommand<TResponse> command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var dispatchMethod = typeof(CommandDispatcher)
            .GetMethod(
                nameof(DispatchAsync),
                BindingFlags.Instance | BindingFlags.NonPublic)!
            .MakeGenericMethod(command.GetType(), typeof(TResponse));

        return (Task<Result<TResponse>>)dispatchMethod.Invoke(this, new object[] { command, cancellationToken })!;
    }

    private Task<Result<TResponse>> DispatchAsync<TCommand, TResponse>(
        TCommand command,
        CancellationToken cancellationToken)
        where TCommand : ICommand<TResponse>
    {
        var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResponse>>();

        var behaviors = _serviceProvider
            .GetServices<ICommandBehavior<TCommand, TResponse>>()
            .Reverse()
            .ToList();

        CommandHandlerDelegate<TResponse> handlerDelegate = ct => handler.Handle(command, ct);

        foreach (var behavior in behaviors)
        {
            var next = handlerDelegate;
            handlerDelegate = ct => behavior.Handle(command, next, ct);
        }

        return handlerDelegate(cancellationToken);
    }
}
