namespace ValenceHub.Persistence.Write.Models.Auth;

public sealed class PersistedRefreshToken
{
    private PersistedRefreshToken()
    {
    }

    private PersistedRefreshToken(
        Guid id,
        Guid userId,
        string tokenHash,
        DateTimeOffset createdAtUtc,
        DateTimeOffset expiresAtUtc)
    {
        Id = id;
        UserId = userId;
        TokenHash = tokenHash;
        CreatedAtUtc = createdAtUtc;
        ExpiresAtUtc = expiresAtUtc;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset ExpiresAtUtc { get; private set; }
    public DateTimeOffset? RevokedAtUtc { get; private set; }

    public static PersistedRefreshToken Create(
        Guid userId,
        string tokenHash,
        DateTimeOffset createdAtUtc,
        DateTimeOffset expiresAtUtc)
        => new(
            Guid.NewGuid(),
            userId,
            tokenHash,
            createdAtUtc,
            expiresAtUtc);

    public void Revoke(DateTimeOffset revokedAtUtc)
    {
        if (RevokedAtUtc is not null)
            return;

        RevokedAtUtc = revokedAtUtc;
    }
}
