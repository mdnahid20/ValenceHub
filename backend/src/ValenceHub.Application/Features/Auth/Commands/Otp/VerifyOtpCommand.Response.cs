using ValenceHub.Domain.Otps.Enums;

namespace ValenceHub.Application.Features.Auth.Commands.Otp;

public sealed record VerifyOtpResponse(
    Guid UserId,
    string Token);
