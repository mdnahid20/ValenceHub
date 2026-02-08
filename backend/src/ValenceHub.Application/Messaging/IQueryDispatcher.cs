using ValenceHub.Application.Abstractions.Queries;
using ValenceHub.Application.Abstractions.Results;

namespace ValenceHub.Application.Messaging;

public interface IQueryDispatcher
{
    Task<Result<TResponse>> DispatchAsync<TResponse>(
        IQuery<TResponse> query,
        CancellationToken cancellationToken = default);
}
