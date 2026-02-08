using MediatR;
using ValenceHub.Application.Abstractions.Results;

namespace ValenceHub.Application.Abstractions.Queries;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}

