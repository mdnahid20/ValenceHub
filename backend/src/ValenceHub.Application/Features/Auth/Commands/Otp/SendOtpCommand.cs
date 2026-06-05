using ValenceHub.Application.Abstractions.Commands;

namespace ValenceHub.Application.Features.Auth.Commands.Otp;

public sealed record SendOtpCommand : ICommand<OtpDeliveryResponse>
{
    public required string Target { get; init; }
    public required string Purpose { get; init; }
    public string? Password { get; init; }
}
