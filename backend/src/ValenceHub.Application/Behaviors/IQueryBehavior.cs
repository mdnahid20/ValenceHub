using System.Threading;
using System.Threading.Tasks;
using ValenceHub.Application.Abstractions.Queries;
using ValenceHub.Application.Abstractions.Results;

namespace ValenceHub.Application.Behaviors;

public delegate Task<Result<TResponse>> QueryHandlerDelegate<TResponse>(CancellationToken cancellationToken);

public interface IQueryBehavior<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> Handle(
        TQuery query,
        QueryHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken);
}
