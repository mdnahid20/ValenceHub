using Ardalis.GuardClauses;
using ValenceHub.Application.Abstractions.Commands;
using ValenceHub.Application.Abstractions.Repositories;
using ValenceHub.Application.Abstractions.Results;
using ValenceHub.Application.Abstractions.Services;
using ValenceHub.Application.Features.Auth.Abstractions;
using ValenceHub.Domain.Common.ValueObjects;
using ValenceHub.Domain.Users;

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
        var user = await ResolveUserAsync(loginId, cancellationToken);

        if (user is null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Result<LoginResponse>.Failure(
                Error.Unauthorized("Auth.InvalidCredentials", "Invalid login credentials."));
        }

        var issuedAtUtc = _clock.UtcNow;
        var accessTokenExpiresAtUtc = _jwtProvider.GetAccessTokenExpiresAt(issuedAtUtc);
        var refreshToken = await _refreshTokenService.IssueAsync(
            user.Id.Value,
            issuedAtUtc,
            cancellationToken);

        var response = new LoginResponse(
            UserId: user.Id.Value,
            AccessToken: _jwtProvider.GenerateAccessToken(user, issuedAtUtc, accessTokenExpiresAtUtc),
            RefreshToken: refreshToken.Token,
            AccessTokenExpiresAtUtc: accessTokenExpiresAtUtc,
            RefreshTokenExpiresAtUtc: refreshToken.ExpiresAtUtc,
            Email: user.Email?.Value,
            PhoneNumber: user.PhoneNumber?.Value);

        return Result<LoginResponse>.Success(response);
    }

    private async Task<User?> ResolveUserAsync(
    string loginId,
    CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(loginId);
        if (emailResult.IsSuccess)
        {
            return await _userRepository.GetByEmailAsync(
                emailResult.Value,
                cancellationToken);
        }

        var phoneResult = PhoneNumber.Create(loginId);
        if (phoneResult.IsSuccess)
        {
            return await _userRepository.GetByPhoneNumberAsync(
                phoneResult.Value,
                cancellationToken);
        }

        return null;
    }
}
