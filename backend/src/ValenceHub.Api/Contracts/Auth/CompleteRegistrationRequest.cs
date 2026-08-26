namespace ValenceHub.Api.Contracts.Auth;

public sealed record CompleteRegistrationRequest(
    Guid UserId,
    string Token);
