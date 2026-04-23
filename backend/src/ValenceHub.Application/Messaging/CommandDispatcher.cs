using MediatR;
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
    private readonly IMediator _mediator;

    public CommandDispatcher(IServiceProvider serviceProvider, IMediator mediator)
    {
        _serviceProvider = serviceProvider;
        _mediator = mediator;
    }

    public Task<Result> Dispatch<TCommand>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        ArgumentNullException.ThrowIfNull(command);
        return DispatchAsync(command, cancellationToken);
    }

    public Task<Result<TResponse>> Dispatch<TResponse>(
        ICommand<TResponse> command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        return _mediator.Send(command, cancellationToken);
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

        CommandHandlerDelegate handlerDelegate = ct => handler.Handle(command, ct);

        foreach (var behavior in behaviors)
        {
            var next = handlerDelegate;
            handlerDelegate = ct => behavior.Handle(command, next, ct);
        }

        return handlerDelegate(cancellationToken);
    }
}
