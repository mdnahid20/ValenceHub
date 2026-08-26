using System.Threading;
using System.Threading.Tasks;
using ValenceHub.Application.Abstractions.Commands;
using ValenceHub.Application.Abstractions.Results;

namespace ValenceHub.Application.Behaviors;

public delegate Task<Result<TResponse>> CommandHandlerDelegate<TResponse>(CancellationToken cancellationToken);

public interface ICommandBehavior<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<Result<TResponse>> Handle(
        TCommand command,
        CommandHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken);
}
