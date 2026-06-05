using FluentValidation;
namespace ValenceHub.Application.Features.Auth.Commands.Otp;

public sealed class SendOtpCommandValidator : AbstractValidator<SendOtpCommand>
{
    public SendOtpCommandValidator()
    {
        RuleFor(x => x.Target)
            .NotEmpty()
            .MaximumLength(320)
            .Must(OtpCommandSupport.IsSupportedTarget)
            .WithMessage("Target must be a valid email address or phone number.");

        RuleFor(x => x.Purpose)
            .NotEmpty()
            .Must(BeValidPurpose)
            .WithMessage("Purpose must be Register, Login, or ResetPassword.");

        RuleFor(x => x.Password)
            .MaximumLength(255)
            .When(x => !string.IsNullOrWhiteSpace(x.Password));
    }

    private static bool BeValidPurpose(string purpose)
        => OtpCommandSupport.TryParsePublicPurpose(purpose, out _);
}
