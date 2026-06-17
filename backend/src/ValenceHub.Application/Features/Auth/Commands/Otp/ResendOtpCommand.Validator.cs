using FluentValidation;
using ValenceHub.Domain.Common.Enums;
namespace ValenceHub.Application.Features.Auth.Commands.Otp;

public sealed class ResendOtpCommandValidator : AbstractValidator<ResendOtpCommand>
{
    public ResendOtpCommandValidator()
    {
        RuleFor(x => x.TargetValue)
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
            .WithMessage("Purpose must be Register, Login, or ResetPassword.");

        RuleFor(x => x)
            .Must(x => OtpCommandSupport.TryResolveTarget(x.TargetValue, out var resolvedChannel, out _) && resolvedChannel == x.Channel)
            .WithMessage("Channel does not match the provided target.");
    }
}
