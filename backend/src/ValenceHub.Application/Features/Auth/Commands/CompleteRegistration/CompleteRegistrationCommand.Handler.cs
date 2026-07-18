using Ardalis.GuardClauses;
using MediatR;
using ValenceHub.Application.Abstractions.Commands;
using ValenceHub.Application.Abstractions.Repositories;
using ValenceHub.Application.Abstractions.Results;
using ValenceHub.Application.Abstractions.Services;
using ValenceHub.Application.Abstractions.Transactions;
using ValenceHub.Application.Features.Auth.Abstractions;
using ValenceHub.Domain.Common.Enums;
using ValenceHub.Domain.Otps.Enums;

namespace ValenceHub.Application.Features.Auth.Commands.CompleteRegistration;

public sealed class CompleteRegistrationCommandHandler : ICommandHandler<CompleteRegistrationCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IOtpService _otpService;
    private readonly IDateTimeOffsetProvider _clock;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteRegistrationCommandHandler(
        IUserRepository userRepository,
        IOtpService otpService,
        IDateTimeOffsetProvider clock,
        IUnitOfWork unitOfWork)
    {
        _userRepository = Guard.Against.Null(userRepository);
        _otpService = Guard.Against.Null(otpService);
        _clock = Guard.Against.Null(clock);
        _unitOfWork = Guard.Against.Null(unitOfWork);
    }

    public async Task<Result<Unit>> Handle(CompleteRegistrationCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result<Unit>.Failure(
                Error.NotFound(
                    "Auth.User.NotFound",
                    "User not found."));
        }

        if (user.IsVerified)
        {
            return Result<Unit>.Failure(
                Error.Conflict(
                    "Auth.User.AlreadyVerified",
                    "User is already verified."));
        }

        var consumeResult = await _otpService.ConsumeVerificationTokenAsync(
            CommunicationChannel.Email,
            user.Email!.Value,
            OtpPurpose.Register,
            request.Token,
            cancellationToken);

        if (consumeResult.IsFailure)
            return Result<Unit>.Failure(consumeResult.Error);

        if (consumeResult.Value != user.Id.Value)
        {
            return Result<Unit>.Failure(
                Error.Validation(
                    "Auth.Register.TokenMismatch",
                    "The registration token does not belong to the provided user."));
        }

        var verifyError = user.Verify(user.Id, _clock.UtcNow);
        if (verifyError is not null)
            return verifyError.ToResult<Unit>();

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}
