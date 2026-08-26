using FluentValidation;
using ValenceHub.Domain.Otps.Enums;

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

        RuleFor(x => x.Channel)
            .IsInEnum()
            .WithMessage("Channel must be a valid CommunicationChannel.");

        RuleFor(x => x.Purpose)
            .IsInEnum()
            .WithMessage("Purpose must be Register, Login, or ForgotPassword.");
    }
}
