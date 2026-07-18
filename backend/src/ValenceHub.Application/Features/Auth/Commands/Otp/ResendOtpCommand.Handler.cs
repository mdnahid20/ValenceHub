using Ardalis.GuardClauses;
using ValenceHub.Application.Abstractions.Commands;
using ValenceHub.Application.Abstractions.Repositories;
using ValenceHub.Application.Abstractions.Results;
using ValenceHub.Application.Abstractions.Services;
using ValenceHub.Application.Features.Auth.Abstractions;
using ValenceHub.Domain.Common.Enums;
using ValenceHub.Domain.Otps.Enums;

namespace ValenceHub.Application.Features.Auth.Commands.Otp;

public sealed class ResendOtpCommandHandler : ICommandHandler<ResendOtpCommand, OtpDeliveryResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IOtpService _otpService;

    public ResendOtpCommandHandler(
        IUserRepository userRepository,
        IOtpService otpService,
        IDateTimeOffsetProvider clock)
    {
        _userRepository = Guard.Against.Null(userRepository);
        _otpService = Guard.Against.Null(otpService);
        Guard.Against.Null(clock);
    }

    public async Task<Result<OtpDeliveryResponse>> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
    {
        if (!OtpCommandSupport.TryResolveTarget(request.TargetValue, out var resolvedChannel, out var targetValue, out _))
        {
            return Result<OtpDeliveryResponse>.Failure(
                Error.Validation(
                    "Otp.InvalidTarget",
                    "Target must be a valid email address or phone number."));
        }

        if (resolvedChannel != request.Channel)
        {
            return Result<OtpDeliveryResponse>.Failure(
                Error.Validation(
                    "Otp.InvalidChannel",
                    "Channel does not match the provided target."));
        }

        if (request.Channel != CommunicationChannel.Email)
        {
            return Result<OtpDeliveryResponse>.Failure(
                Error.NotSupported(
                    "Otp.PhonePaused",
                    "Phone OTP is currently paused."));
        }

        if (request.Purpose != OtpPurpose.Register)
        {
            //TODO : Later we will do work on other OTP purposes like password reset, MFA etc. For now, we only support registration OTPs.
            return Result<OtpDeliveryResponse>.Failure(
                Error.NotSupported(
                    "Otp.OtherPurpose",
                    "Only registration OTPs can be resent."));
        }

        var registerUser = await _userRepository.GetByTargetAsync(request.Channel, targetValue, cancellationToken);
        if (registerUser is null)
        {
            return Result<OtpDeliveryResponse>.Failure(
                Error.NotFound(
                    "Auth.User.NotFound",
                    "User not found for registration verification."));
        }

        if (registerUser.IsVerified)
        {
            return Result<OtpDeliveryResponse>.Failure(
                Error.Conflict(
                    "Auth.User.AlreadyVerified",
                    "User is already verified."));
        }

        var resendResult = await _otpService.ResendAsync(request.Channel, targetValue, request.Purpose, cancellationToken);
        return resendResult.IsFailure
            ? Result<OtpDeliveryResponse>.Failure(resendResult.Error)
            : Result<OtpDeliveryResponse>.Success(new OtpDeliveryResponse(true, registerUser.Id.Value));
    }
}
