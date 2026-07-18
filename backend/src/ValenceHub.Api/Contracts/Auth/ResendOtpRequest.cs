namespace ValenceHub.Api.Contracts.Auth;

public sealed record ResendOtpRequest(
    string TargetValue,
    short Channel,
    short Purpose);
