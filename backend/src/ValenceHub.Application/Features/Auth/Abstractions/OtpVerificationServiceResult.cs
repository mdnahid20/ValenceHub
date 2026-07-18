namespace ValenceHub.Application.Features.Auth.Abstractions;

public sealed record OtpVerificationServiceResult(
    Guid UserId,
    string? Token = null);
