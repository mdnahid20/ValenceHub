namespace ValenceHub.Api.Contracts.Auth;

public sealed record SendOtpRequest(
    string Target,
    string Purpose,
    string? Password = null);
