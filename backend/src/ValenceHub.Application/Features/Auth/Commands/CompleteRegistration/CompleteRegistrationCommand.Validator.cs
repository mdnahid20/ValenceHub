using FluentValidation;

namespace ValenceHub.Application.Features.Auth.Commands.CompleteRegistration;

public sealed class CompleteRegistrationCommandValidator : AbstractValidator<CompleteRegistrationCommand>
{
    public CompleteRegistrationCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.Token)
            .NotEmpty()
            .MinimumLength(20)
            .WithMessage("Registration token is invalid.");
    }
}
