namespace ValenceHub.Application.Abstractions.Repositories;

/// <summary>
/// Minimal data required to verify a password and issue tokens at login.
/// </summary>
public sealed record UserLoginCredentials(Guid UserId, string PasswordHash, bool IsVerified);
