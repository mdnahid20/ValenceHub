namespace ValenceHub.Application.Features.Auth.Commands.Login;

public sealed record LoginResponse(
    Guid UserId,
    string AccessToken,
    string RefreshToken,
    DateTimeOffset AccessTokenExpiresAtUtc,
    DateTimeOffset RefreshTokenExpiresAtUtc,
    string? Email,
    string? PhoneNumber);
