using ValenceHub.Application.Abstractions.Commands;
using ValenceHub.Application.Abstractions.Results;

namespace ValenceHub.Application.Messaging;

public interface ICommandDispatcher
{
    Task<Result> Dispatch<TCommand>(
       TCommand command,
       CancellationToken cancellationToken = default)
       where TCommand : ICommand;

    Task<Result<TResponse>> Dispatch<TResponse>(
       ICommand<TResponse> command,
       CancellationToken cancellationToken = default);
}
