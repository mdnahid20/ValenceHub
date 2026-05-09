using ValenceHub.Domain.Users;

namespace ValenceHub.Application.Features.Auth.Abstractions;

public interface IJwtProvider
{
    string GenerateAccessToken(User user, DateTimeOffset issuedAtUtc, DateTimeOffset expiresAtUtc);
    DateTimeOffset GetAccessTokenExpiresAt(DateTimeOffset utcNow);
}
