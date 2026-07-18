namespace ValenceHub.Application.Features.Auth.Commands.Otp;

public sealed record OtpDeliveryResponse(
    bool Success,
    Guid UserId);
