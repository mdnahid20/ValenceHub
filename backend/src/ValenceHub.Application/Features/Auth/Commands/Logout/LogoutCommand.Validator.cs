using FluentValidation;

namespace ValenceHub.Application.Features.Auth.Commands.Logout;

public sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    private const int MaxRefreshTokenLength = 512;

    public LogoutCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required")
            .MaximumLength(MaxRefreshTokenLength)
            .WithMessage($"Refresh token cannot exceed {MaxRefreshTokenLength} characters");
    }
}
