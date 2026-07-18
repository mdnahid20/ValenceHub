using Ardalis.GuardClauses;
using ValenceHub.Application.Abstractions.Commands;
using ValenceHub.Application.Abstractions.Repositories;
using ValenceHub.Application.Abstractions.Results;
using ValenceHub.Application.Abstractions.Services;
using ValenceHub.Application.Features.Auth.Abstractions;
using ValenceHub.Domain.Common.Enums;
using ValenceHub.Domain.Otps.Enums;

namespace ValenceHub.Application.Features.Auth.Commands.Otp;

public sealed class SendOtpCommandHandler : ICommandHandler<SendOtpCommand, OtpDeliveryResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IOtpService _otpService;

    public SendOtpCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IOtpService otpService,
        IDateTimeOffsetProvider clock)
    {
        _userRepository = Guard.Against.Null(userRepository);
        _passwordHasher = Guard.Against.Null(passwordHasher);
        _otpService = Guard.Against.Null(otpService);
        Guard.Against.Null(clock);
    }

    public async Task<Result<OtpDeliveryResponse>> Handle(SendOtpCommand request, CancellationToken cancellationToken)
    {
        if (!OtpCommandSupport.TryResolveTarget(request.Target, out var resolvedChannel, out var targetValue, out _))
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

        var purpose = request.Purpose;

        if (purpose == OtpPurpose.Register)
        {
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

            var sendResult = await _otpService.IssueAsync(
                registerUser.Id.Value,
                request.Channel,
                targetValue,
                purpose,
                cancellationToken);

            return sendResult.IsFailure
                ? Result<OtpDeliveryResponse>.Failure(sendResult.Error)
                : Result<OtpDeliveryResponse>.Success(new OtpDeliveryResponse(true, registerUser.Id.Value));
        }
        else if (purpose == OtpPurpose.Login)
        {
            var loginUser = await _userRepository.GetByTargetAsync(request.Channel, targetValue, cancellationToken);
            if (loginUser is null)
            {
                return Result<OtpDeliveryResponse>.Failure(
                    Error.Unauthorized(
                        "Auth.InvalidCredentials",
                        "Invalid login credentials."));
            }

            if (!loginUser.IsVerified)
            {
                return Result<OtpDeliveryResponse>.Failure(
                    Error.Forbidden(
                        "Auth.User.NotVerified",
                        "Account verification is required before login."));
            }

            var sendResult = await _otpService.IssueAsync(
                loginUser.Id.Value,
                request.Channel,
                targetValue,
                purpose,
                cancellationToken);

            return sendResult.IsFailure
                ? Result<OtpDeliveryResponse>.Failure(sendResult.Error)
                : Result<OtpDeliveryResponse>.Success(new OtpDeliveryResponse(true, loginUser.Id.Value));
        }
        else if (purpose == OtpPurpose.ForgotPassword)
        {
            var resetUser = await _userRepository.GetByTargetAsync(request.Channel, targetValue, cancellationToken);
            if (resetUser is null || !resetUser.IsVerified)
            {
                return Result<OtpDeliveryResponse>.Failure(
                    Error.NotFound(
                        "Auth.User.NotFound",
                        "User not found for password reset."));
            }

            var resetResult = await _otpService.IssueAsync(
                resetUser.Id.Value,
                request.Channel,
                targetValue,
                purpose,
                cancellationToken);

            return resetResult.IsFailure
                ? Result<OtpDeliveryResponse>.Failure(resetResult.Error)
                : Result<OtpDeliveryResponse>.Success(new OtpDeliveryResponse(true, resetUser.Id.Value));
        }
        else
        {
            return Result<OtpDeliveryResponse>.Failure(
                Error.Validation(
                    "Otp.UnsupportedPurpose",
                    $"OTP purpose '{purpose}' is not currently supported."));
        }
    }
}
