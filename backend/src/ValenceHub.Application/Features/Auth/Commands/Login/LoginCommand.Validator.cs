using FluentValidation;

namespace ValenceHub.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.LoginId)
            .NotEmpty()
            .WithMessage("Login ID is required")
            .MaximumLength(320)
            .WithMessage("Login ID cannot exceed 320 characters");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MaximumLength(255)
            .WithMessage("Password cannot exceed 255 characters");
    }
}
