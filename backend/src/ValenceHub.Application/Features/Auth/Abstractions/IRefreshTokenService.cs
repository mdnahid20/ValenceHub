using ValenceHub.Application.Abstractions.Results;

namespace ValenceHub.Application.Features.Auth.Abstractions;

public interface IRefreshTokenService
{
    Task<RefreshTokenIssueResult> IssueAsync(
        Guid userId,
        DateTimeOffset utcNow,
        CancellationToken cancellationToken = default);

    Task<Guid?> GetUserIdAsync(
        string refreshToken,
        DateTimeOffset utcNow,
        CancellationToken cancellationToken = default);

    Task<Result> RevokeAsync(
        string refreshToken,
        DateTimeOffset revokedAtUtc,
        CancellationToken cancellationToken = default);

    Task RevokeAllByUserIdAsync(
        Guid userId,
        DateTimeOffset revokedAtUtc,
        CancellationToken cancellationToken = default);
}

public sealed record RefreshTokenIssueResult(
    string Token,
    DateTimeOffset ExpiresAtUtc);
