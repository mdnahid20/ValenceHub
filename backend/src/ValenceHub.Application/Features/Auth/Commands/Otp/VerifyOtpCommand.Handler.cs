using Ardalis.GuardClauses;
using ValenceHub.Application.Abstractions.Commands;
using ValenceHub.Application.Abstractions.Results;
using ValenceHub.Application.Features.Auth.Abstractions;
using ValenceHub.Domain.Common.Enums;
using ValenceHub.Domain.Otps.Enums;

namespace ValenceHub.Application.Features.Auth.Commands.Otp;

public sealed class VerifyOtpCommandHandler : ICommandHandler<VerifyOtpCommand, VerifyOtpResponse>
{
    private readonly IOtpService _otpService;

    public VerifyOtpCommandHandler(IOtpService otpService)
    {
        _otpService = Guard.Against.Null(otpService);
    }

    public async Task<Result<VerifyOtpResponse>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        if (!OtpCommandSupport.TryResolveTarget(request.Target, out var resolvedChannel, out var normalizedTarget))
        {
            return Result<VerifyOtpResponse>.Failure(
                Error.Validation(
                    "Otp.InvalidTarget",
                    "Target must be a valid email address or phone number."));
        }

        if (resolvedChannel != request.Channel)
        {
            return Result<VerifyOtpResponse>.Failure(
                Error.Validation(
                    "Otp.InvalidChannel",
                    "Channel does not match the provided target."));
        }

        if (request.Channel != CommunicationChannel.Email)
        {
            return Result<VerifyOtpResponse>.Failure(
                Error.NotSupported(
                    "Otp.PhonePaused",
                    "Phone OTP verification is currently paused."));
        }

        var verificationResult = await _otpService.VerifyAsync(
            request.Channel,
            normalizedTarget,
            request.Purpose,
            request.Code,
            cancellationToken);

        if (verificationResult.IsFailure)
            return Result<VerifyOtpResponse>.Failure(verificationResult.Error);

        var payload = verificationResult.Value!;

        return Result<VerifyOtpResponse>.Success(
            new VerifyOtpResponse(payload.UserId, payload.Token!));
    }
}
