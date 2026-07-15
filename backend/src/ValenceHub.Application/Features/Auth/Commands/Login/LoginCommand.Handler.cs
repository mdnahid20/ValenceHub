using Ardalis.GuardClauses;
using ValenceHub.Application.Abstractions.Commands;
using ValenceHub.Application.Abstractions.Repositories;
using ValenceHub.Application.Abstractions.Results;
using ValenceHub.Application.Abstractions.Services;
using ValenceHub.Application.Features.Auth.Abstractions;
using ValenceHub.Application.Features.Auth.Commands.Otp;
using ValenceHub.Domain.Common.Enums;

namespace ValenceHub.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IDateTimeOffsetProvider _clock;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider,
        IRefreshTokenService refreshTokenService,
        IDateTimeOffsetProvider clock)
    {
        _userRepository = Guard.Against.Null(userRepository);
        _passwordHasher = Guard.Against.Null(passwordHasher);
        _jwtProvider = Guard.Against.Null(jwtProvider);
        _refreshTokenService = Guard.Against.Null(refreshTokenService);
        _clock = Guard.Against.Null(clock);
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var loginId = request.LoginId.Trim();
        if (!OtpCommandSupport.TryResolveTarget(loginId, out var targetType, out var normalizedLoginId))
        {
            return Result<LoginResponse>.Failure(
                Error.Unauthorized("Auth.InvalidCredentials", "Invalid login credentials."));
        }

        if (targetType != CommunicationChannel.Email)
        {
            return Result<LoginResponse>.Failure(
                Error.NotSupported(
                    "Auth.Login.PhonePaused",
                    "Phone login is currently paused."));
        }

        var credentials = await _userRepository.GetCredentialsByTargetAsync(targetType, normalizedLoginId, cancellationToken);

        if (credentials is null
            || !_passwordHasher.VerifyPassword(request.Password, credentials.PasswordHash))
        {
            return Result<LoginResponse>.Failure(
                Error.Unauthorized("Auth.InvalidCredentials", "Invalid login credentials."));
        }

        if (!credentials.IsVerified)
        {
            return Result<LoginResponse>.Failure(
                Error.Forbidden(
                    "Auth.User.NotVerified",
                    "Account verification is required before login."));
        }

        var issuedAtUtc = _clock.UtcNow;
        var accessTokenExpiresAtUtc = _jwtProvider.GetAccessTokenExpiresAt(issuedAtUtc);
        var accessToken = _jwtProvider.GenerateAccessToken(credentials.UserId, issuedAtUtc, accessTokenExpiresAtUtc);   
        var refreshToken = await _refreshTokenService.IssueAsync(
            credentials.UserId,
            issuedAtUtc,
            cancellationToken);

        var response = new LoginResponse(
            AccessToken: accessToken,
            RefreshToken: refreshToken.Token,
            AccessTokenExpiresAtUtc: accessTokenExpiresAtUtc,
            RefreshTokenExpiresAtUtc: refreshToken.ExpiresAtUtc);

        return Result<LoginResponse>.Success(response);
    }
}
