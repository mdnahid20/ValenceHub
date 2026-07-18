using FluentValidation;

namespace ValenceHub.Application.Features.Auth.Commands.ChangePassword;

public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    private const int MinPasswordLength = 8;

    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .WithMessage("Current password is required")
            .MaximumLength(255)
            .WithMessage("Current password cannot exceed 255 characters");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("New password is required")
            .MinimumLength(MinPasswordLength)
            .WithMessage($"New password must be at least {MinPasswordLength} characters")
            .Matches("[A-Z]")
            .WithMessage("New password must contain at least one uppercase letter")
            .Matches("[a-z]")
            .WithMessage("New password must contain at least one lowercase letter")
            .Matches("[0-9]")
            .WithMessage("New password must contain at least one digit")
            .Matches("[!@#$%^&*]")
            .WithMessage("New password must contain at least one special character")
            .MaximumLength(255)
            .WithMessage("New password cannot exceed 255 characters");

        RuleFor(x => x)
            .Must(x => !string.Equals(x.CurrentPassword, x.NewPassword, StringComparison.Ordinal))
            .WithMessage("New password must be different from current password.");
    }
}
