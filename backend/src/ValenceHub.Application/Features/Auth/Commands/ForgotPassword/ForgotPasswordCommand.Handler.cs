using Ardalis.GuardClauses;
using MediatR;
using ValenceHub.Application.Abstractions.Commands;
using ValenceHub.Application.Abstractions.Repositories;
using ValenceHub.Application.Abstractions.Results;
using ValenceHub.Application.Abstractions.Services;
using ValenceHub.Application.Abstractions.Transactions;
using ValenceHub.Application.Features.Auth.Abstractions;
using ValenceHub.Domain.Common.Enums;
using ValenceHub.Domain.Common.ValueObjects;
using ValenceHub.Domain.Otps;
using ValenceHub.Domain.Otps.Enums;

namespace ValenceHub.Application.Features.Auth.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IOtpService _otpService;
    private readonly IDateTimeOffsetProvider _clock;
    private readonly IUnitOfWork _unitOfWork;

    public ForgotPasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IOtpService otpService,
        IDateTimeOffsetProvider clock,
        IUnitOfWork unitOfWork)
    {
        _userRepository = Guard.Against.Null(userRepository);
        _passwordHasher = Guard.Against.Null(passwordHasher);
        _otpService = Guard.Against.Null(otpService);
        _clock = Guard.Against.Null(clock);
        _unitOfWork = Guard.Against.Null(unitOfWork);
    }

    public async Task<Result<Unit>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var password = Password.Create(request.NewPassword);
        if (password.IsFailure)
            return password.Error.ToResult<Unit>();

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result<Unit>.Failure(
                Error.NotFound(
                    "Auth.User.NotFound",
                    "User not found."));
        }

        var userTargetValue = user.Email?.Value ?? user.PhoneNumber?.Value;
        var userTargetType = user.Email is not null
            ? CommunicationChannel.Email
            : CommunicationChannel.SMS;

        if (string.IsNullOrWhiteSpace(userTargetValue))
        {
            return Result<Unit>.Failure(
                Error.Validation(
                    "Auth.ResetPassword.TargetMissing",
                    "The user does not have a valid OTP target."));
        }

        if (userTargetType != CommunicationChannel.Email)
        {
            return Result<Unit>.Failure(
                Error.NotSupported(
                    "Auth.ResetPassword.PhonePaused",
                    "Phone password reset is currently paused."));
        }

        var userId = await _otpService.ConsumeVerificationTokenAsync(
            userTargetType,
            userTargetValue,
            OtpPurpose.ForgotPassword,
            request.VerificationToken,
            cancellationToken);

        if (userId.IsFailure)
        {
            return Result<Unit>.Failure(userId.Error);
        }

        if (userId.Value != user.Id.Value)
        {
            return Result<Unit>.Failure(
                Error.Validation(
                    "Auth.ResetPassword.UserMismatch",
                    "The verification token does not belong to the provided user."));
        }

        var updateError = user.UpdatePasswordHash(
            _passwordHasher.HashPassword(password.Value.Value),
            user.Id,
            _clock.UtcNow);

        if (updateError is not null)
            return updateError.ToResult<Unit>();

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}
