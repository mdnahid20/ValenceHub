namespace ValenceHub.Api.Contracts.Auth;

public sealed record RegisterRequest(
    string? Email,
    string? PhoneNumber,
    string Password);
