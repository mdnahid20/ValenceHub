using MediatR;
using ValenceHub.Application.Abstractions.Results;

namespace ValenceHub.Application.Abstractions.Commands;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}

