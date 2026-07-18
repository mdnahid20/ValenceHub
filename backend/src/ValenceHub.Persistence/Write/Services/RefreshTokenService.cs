using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using ValenceHub.Application.Abstractions.Results;
using ValenceHub.Application.Features.Auth.Abstractions;
using ValenceHub.Persistence.Write.Contexts;
using ValenceHub.Persistence.Write.Models.Auth;

namespace ValenceHub.Persistence.Write.Services;

public sealed class RefreshTokenService : IRefreshTokenService
{
    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);
    private readonly ValenceHubWriteDbContext _context;

    public RefreshTokenService(ValenceHubWriteDbContext context)
    {
        _context = context;
    }

    public Task<RefreshTokenIssueResult> IssueAsync(
        Guid userId,
        DateTimeOffset utcNow,
        CancellationToken cancellationToken = default)
    {
        var token = CreateToken();
        var tokenHash = ComputeHash(token);
        var expiresAtUtc = utcNow.Add(RefreshTokenLifetime);

        _context.RefreshTokens.Add(
            PersistedRefreshToken.Create(
                userId,
                tokenHash,
                utcNow,
                expiresAtUtc));

        return Task.FromResult(new RefreshTokenIssueResult(token, expiresAtUtc));
    }

    public async Task<Guid?> GetUserIdAsync(
        string refreshToken,
        DateTimeOffset utcNow,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return null;

        var tokenHash = ComputeHash(refreshToken);

        var session = await _context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

        if (session is null || session.RevokedAtUtc is not null || session.ExpiresAtUtc <= utcNow)
            return null;

        return session.UserId;
    }

    public async Task<Result> RevokeAsync(
        string refreshToken,
        DateTimeOffset revokedAtUtc,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Result.Failure(
                Error.Validation(
                    "Auth.RefreshToken.Required",
                    "Refresh token is required."));
        }

        var tokenHash = ComputeHash(refreshToken);

        var session = await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

        if (session is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "Auth.RefreshToken.NotFound",
                    "Refresh token was not found."));
        }

        if (session.RevokedAtUtc is not null)
        {
            return Result.Failure(
                Error.Conflict(
                    "Auth.RefreshToken.AlreadyRevoked",
                    "Refresh token has already been revoked."));
        }

        if (session.ExpiresAtUtc <= revokedAtUtc)
        {
            return Result.Failure(
                Error.Unauthorized(
                    "Auth.RefreshToken.Expired",
                    "Refresh token is expired."));
        }

        session.Revoke(revokedAtUtc);

        return Result.Success();
    }

    public async Task RevokeAllByUserIdAsync(
        Guid userId,
        DateTimeOffset revokedAtUtc,
        CancellationToken cancellationToken = default)
    {
        var sessions = await _context.RefreshTokens
            .Where(x => x.UserId == userId && x.RevokedAtUtc == null)
            .ToListAsync(cancellationToken);

        foreach (var session in sessions)
        {
            session.Revoke(revokedAtUtc);
        }
    }

    private static string CreateToken()
        => Base64UrlEncode(RandomNumberGenerator.GetBytes(64));

    private static string ComputeHash(string token)
        => Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token)));

    private static string Base64UrlEncode(byte[] value)
        => Convert.ToBase64String(value)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
}
