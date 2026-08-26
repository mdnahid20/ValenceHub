namespace ValenceHub.Api.Contracts.Auth;

public sealed record LoginRequest(
    string LoginId,
    string Password);