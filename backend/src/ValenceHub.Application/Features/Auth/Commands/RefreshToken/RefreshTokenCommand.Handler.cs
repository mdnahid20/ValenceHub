using Ardalis.GuardClauses;
using ValenceHub.Application.Abstractions.Commands;
using ValenceHub.Application.Abstractions.Repositories;
using ValenceHub.Application.Abstractions.Results;
using ValenceHub.Application.Abstractions.Services;
using ValenceHub.Application.Features.Auth.Abstractions;

namespace ValenceHub.Application.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtProvider _jwtProvider;
    private readonly IDateTimeOffsetProvider _clock;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenService refreshTokenService,
        IJwtProvider jwtProvider,
        IDateTimeOffsetProvider clock)
    {
        _userRepository = Guard.Against.Null(userRepository);
        _refreshTokenService = Guard.Against.Null(refreshTokenService);
        _jwtProvider = Guard.Against.Null(jwtProvider);
        _clock = Guard.Against.Null(clock);
    }

    public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var now = _clock.UtcNow;
        var userId = await _refreshTokenService.GetUserIdAsync(
            request.RefreshToken,
            now,
            cancellationToken);

        if (userId is null)
        {
            return Result<RefreshTokenResponse>.Failure(
                Error.Unauthorized("Auth.InvalidRefreshToken", "Refresh token is invalid or expired."));
        }

        var userExists = await _userRepository.ExistsAsync(userId.Value, cancellationToken);
        if (!userExists)
        {
            return Result<RefreshTokenResponse>.Failure(
                Error.Unauthorized("Auth.InvalidRefreshToken", "Refresh token is invalid or expired."));
        }

        await _refreshTokenService.RevokeAsync(
            request.RefreshToken,
            now,
            cancellationToken);

        var accessTokenExpiresAtUtc = _jwtProvider.GetAccessTokenExpiresAt(now);
        var refreshToken = await _refreshTokenService.IssueAsync(
            userId.Value,
            now,
            cancellationToken);

        var response = new RefreshTokenResponse(
            UserId: userId.Value,
            AccessToken: _jwtProvider.GenerateAccessToken(userId.Value, now, accessTokenExpiresAtUtc),
            RefreshToken: refreshToken.Token,
            AccessTokenExpiresAtUtc: accessTokenExpiresAtUtc,
            RefreshTokenExpiresAtUtc: refreshToken.ExpiresAtUtc);

        return Result<RefreshTokenResponse>.Success(response);
    }
}
