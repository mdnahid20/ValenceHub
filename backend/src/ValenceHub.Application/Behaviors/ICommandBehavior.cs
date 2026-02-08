using System.Threading;
using System.Threading.Tasks;
using ValenceHub.Application.Abstractions.Commands;
using ValenceHub.Application.Abstractions.Results;

namespace ValenceHub.Application.Behaviors;

public delegate Task<Result> CommandHandlerDelegate();

public interface ICommandBehavior<TCommand>
    where TCommand : ICommand
{
    Task<Result> Handle(
        TCommand command,
        CommandHandlerDelegate next,
        CancellationToken cancellationToken);
}
