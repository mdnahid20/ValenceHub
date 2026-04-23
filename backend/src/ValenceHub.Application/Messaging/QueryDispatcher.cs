using System;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ValenceHub.Application.Abstractions.Queries;
using ValenceHub.Application.Abstractions.Results;

namespace ValenceHub.Application.Messaging;

internal sealed class QueryDispatcher : IQueryDispatcher
{
    private readonly IMediator _mediator;

    public QueryDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<Result<TResponse>> DispatchAsync<TResponse>(
        IQuery<TResponse> query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var queryType = query.GetType();
        var requestHandlerType = typeof(IRequestHandler<,>)
            .MakeGenericType(queryType, typeof(Result<TResponse>));

        var method = typeof(IMediator)
            .GetMethod(nameof(IMediator.Send), 1, new[] { typeof(IRequest<>), typeof(CancellationToken) })
            ?.MakeGenericMethod(typeof(Result<TResponse>));

        if (method == null)
        {
            throw new InvalidOperationException($"Cannot resolve query handler for {queryType.Name}");
        }

        return (Task<Result<TResponse>>)method.Invoke(_mediator, new object[] { query, cancellationToken })!;
    }
}
