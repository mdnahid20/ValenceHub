namespace ValenceHub.Application.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenResponse(
    Guid UserId,
    string AccessToken,
    string RefreshToken,
    DateTimeOffset AccessTokenExpiresAtUtc,
    DateTimeOffset RefreshTokenExpiresAtUtc);
