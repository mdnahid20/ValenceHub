using Ardalis.GuardClauses;
using MediatR;
using ValenceHub.Application.Abstractions.Commands;
using ValenceHub.Application.Abstractions.Repositories;
using ValenceHub.Application.Abstractions.Results;
using ValenceHub.Application.Abstractions.Services;
using ValenceHub.Application.Abstractions.Transactions;
using ValenceHub.Application.Features.Auth.Abstractions;

namespace ValenceHub.Application.Features.Auth.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ICurrentUser _currentUser;
    private readonly IDateTimeOffsetProvider _clock;
    private readonly IUnitOfWork _unitOfWork;

    public ChangePasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IRefreshTokenService refreshTokenService,
        ICurrentUser currentUser,
        IDateTimeOffsetProvider clock,
        IUnitOfWork unitOfWork)
    {
        _userRepository = Guard.Against.Null(userRepository);
        _passwordHasher = Guard.Against.Null(passwordHasher);
        _refreshTokenService = Guard.Against.Null(refreshTokenService);
        _currentUser = Guard.Against.Null(currentUser);
        _clock = Guard.Against.Null(clock);
        _unitOfWork = Guard.Against.Null(unitOfWork);
    }

    public async Task<Result<Unit>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
        {
            return Result<Unit>.Failure(
                Error.Unauthorized(
                    "Auth.Unauthorized",
                    "Authentication is required."));
        }

        var user = await _userRepository.GetByIdAsync(_currentUser.UserId.Value, cancellationToken);
        if (user is null)
        {
            return Result<Unit>.Failure(
                Error.NotFound(
                    "Auth.User.NotFound",
                    "User not found."));
        }

        if (!_passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
        {
            return Result<Unit>.Failure(
                Error.Unauthorized(
                    "Auth.InvalidCurrentPassword",
                    "Current password is incorrect."));
        }

        var updateError = user.UpdatePasswordHash(
            _passwordHasher.HashPassword(request.NewPassword),
            user.Id,
            _clock.UtcNow);

        if (updateError is not null)
            return updateError.ToResult<Unit>();

        _userRepository.Update(user);
        await _refreshTokenService.RevokeAllByUserIdAsync(user.Id.Value, _clock.UtcNow, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}
