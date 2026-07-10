namespace ValenceHub.Api.Contracts.Auth;

public sealed record SendOtpRequest(
    string Target,
    short Channel,
    short Purpose);
