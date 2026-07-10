using FluentValidation;
using ValenceHub.Domain.Common.Enums;
using ValenceHub.Domain.Otps.Enums;

namespace ValenceHub.Application.Features.Auth.Commands.Otp;

public sealed class VerifyOtpCommandValidator : AbstractValidator<VerifyOtpCommand>
{
    public VerifyOtpCommandValidator()
    {
        RuleFor(x => x.Target)
            .NotEmpty()
            .MaximumLength(320)
            .Must(OtpCommandSupport.IsSupportedTarget)
            .WithMessage("Target must be a valid email address or phone number.");

        RuleFor(x => x.Channel)
            .IsInEnum()
            .Must(channel => channel is CommunicationChannel.Email or CommunicationChannel.SMS)
            .WithMessage("Channel must be Email or SMS.");

        RuleFor(x => x.Purpose)
            .IsInEnum()
            .Must(OtpCommandSupport.IsSupportedPublicPurpose)
            .WithMessage("Purpose must be Register, Login, or ForgotPassword.");

        RuleFor(x => x)
            .Must(x => OtpCommandSupport.TryResolveTarget(x.Target, out var resolvedChannel, out _) && resolvedChannel == x.Channel)
            .WithMessage("Channel does not match the provided target.");

        RuleFor(x => x.Code)
            .NotEmpty()
            .Length(6)
            .Matches("^[0-9]{6}$");
    }
}
