namespace ValenceHub.Api.Contracts.Auth;

public sealed record ForgotPasswordRequest(
    Guid UserId,
    string VerificationToken,
    string NewPassword);
