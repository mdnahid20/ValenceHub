using ValenceHub.Application.Abstractions.Commands;
using ValenceHub.Application.Abstractions.Results;

namespace ValenceHub.Application.Messaging;

public interface ICommandDispatcher
{
    Task<Result<TResponse>> Dispatch<TResponse>(
       ICommand<TResponse> command,
       CancellationToken cancellationToken = default);
}
