using FluentValidation;

namespace ValenceHub.Application.Features.Auth.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    private const int MinPasswordLength = 8;

    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.VerificationToken)
            .NotEmpty()
            .Must(BeValidOtpOrVerificationToken)
            .WithMessage("Verification token is invalid.");

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
    }

    private static bool BeValidOtpOrVerificationToken(string verificationToken)
    {
        var trimmed = verificationToken?.Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
            return false;

        return (trimmed.Length == 6 && trimmed.All(char.IsDigit)) || trimmed.Length >= 20;
    }
}
