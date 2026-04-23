using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ValenceHub.Application.Abstractions.Commands;
using ValenceHub.Application.Abstractions.Results;

namespace ValenceHub.Application.Behaviors;

public sealed class ValidationBehavior<TCommand> : ICommandBehavior<TCommand>
    where TCommand : ICommand
{
    private readonly IEnumerable<IValidator<TCommand>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TCommand>> validators)
    {
        _validators = validators;
    }

    public async Task<Result> Handle(
        TCommand command,
        CommandHandlerDelegate next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TCommand>(command);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count != 0)
        {
            var message = string.Join(", ",
                failures.Select(f => f.ErrorMessage));

            return Result.Failure(new Error(
                "Validation",
                message));
        }

        return await next(cancellationToken);
    }
}
