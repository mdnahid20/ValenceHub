using FluentValidation;

namespace ValenceHub.Application.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    private const int MaxRefreshTokenLength = 512;

    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required")
            .MaximumLength(MaxRefreshTokenLength)
            .WithMessage($"Refresh token cannot exceed {MaxRefreshTokenLength} characters");
    }
}
