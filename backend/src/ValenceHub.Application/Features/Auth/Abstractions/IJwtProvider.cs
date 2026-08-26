namespace ValenceHub.Application.Features.Auth.Abstractions;

public interface IJwtProvider
{
    string GenerateAccessToken(Guid userId, DateTimeOffset issuedAtUtc, DateTimeOffset expiresAtUtc);
    DateTimeOffset GetAccessTokenExpiresAt(DateTimeOffset utcNow);
}
