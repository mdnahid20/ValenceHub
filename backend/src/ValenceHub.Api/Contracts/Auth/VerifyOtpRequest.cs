namespace ValenceHub.Api.Contracts.Auth;

public sealed record VerifyOtpRequest(
    string Target,
    short Channel,  
    short Purpose,
    string Code);
