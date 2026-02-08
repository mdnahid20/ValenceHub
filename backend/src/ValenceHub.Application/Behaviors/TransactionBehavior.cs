using ValenceHub.Application.Abstractions.Commands;
using ValenceHub.Application.Abstractions.Results;
using ValenceHub.Application.Abstractions.Transactions;

namespace ValenceHub.Application.Behaviors;

public sealed class TransactionBehavior<TCommand>
    : ICommandBehavior<TCommand>
    where TCommand : ICommand
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionBehavior(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        TCommand command,
        CommandHandlerDelegate next,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var result = await next();

            if (result.IsSuccess)
            {
                await _unitOfWork.CommitAsync(cancellationToken);
            }
            else
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
            }

            return result;
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
