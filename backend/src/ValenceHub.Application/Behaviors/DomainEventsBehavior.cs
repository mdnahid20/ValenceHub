using Microsoft.Extensions.DependencyInjection;
using ValenceHub.Application.Abstractions.Commands;
using ValenceHub.Application.Abstractions.Results;
using ValenceHub.Application.Abstractions.Transactions;
using ValenceHub.Application.Messaging;
using ValenceHub.Domain.Abstractions;
using ValenceHub.Domain.Events;

namespace ValenceHub.Application.Behaviors;

public sealed class DomainEventsBehavior<TCommand, TResponse>
    : ICommandBehavior<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public DomainEventsBehavior(
        IUnitOfWork unitOfWork,
        IDomainEventDispatcher domainEventDispatcher)
    {
        _unitOfWork = unitOfWork;
        _domainEventDispatcher = domainEventDispatcher;
    }

    public async Task<Result<TResponse>> Handle(
        TCommand command,
        CommandHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var result = await next(cancellationToken);

        if (result.IsFailure)
        {
            return result;
        }

        var aggregateRoots = _unitOfWork.GetAggregateRoots();

        var domainEvents = new List<DomainEvent>();
        foreach (var root in aggregateRoots)
        {
            if (root is IHasDomainEvents entity)
            {
                domainEvents.AddRange(entity.GetDomainEvents());
            }
        }

        foreach (var domainEvent in domainEvents)
        {
            await _domainEventDispatcher.DispatchAsync(
                new[] { domainEvent },
                cancellationToken);
        }

        return result;
    }
}
