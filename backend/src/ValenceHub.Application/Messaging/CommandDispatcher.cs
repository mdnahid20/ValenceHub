using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
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

    public Task<Result> Dispatch<TCommand>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        ArgumentNullException.ThrowIfNull(command);
        return DispatchAsync(command, cancellationToken);
    }

    private Task<Result> DispatchAsync<TCommand>(
        TCommand command,
        CancellationToken cancellationToken)
        where TCommand : ICommand
    {
        var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();

        var behaviors = _serviceProvider
            .GetServices<ICommandBehavior<TCommand>>()
            .Reverse()
            .ToList();

        CommandHandlerDelegate handlerDelegate =
            () => handler.Handle(command, cancellationToken);

        foreach (var behavior in behaviors)
        {
            var next = handlerDelegate;
            handlerDelegate = () => behavior.Handle(command, next, cancellationToken);
        }

        return handlerDelegate();
    }
}
