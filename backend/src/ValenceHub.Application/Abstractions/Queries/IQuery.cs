using MediatR;
using ValenceHub.Application.Abstractions.Results;

namespace ValenceHub.Application.Abstractions.Queries;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}

