using ValenceHub.Application.Abstractions.Commands;
using ValenceHub.Domain.Common.Enums;
using ValenceHub.Domain.Otps.Enums;

namespace ValenceHub.Application.Features.Auth.Commands.Otp;

public sealed record ResendOtpCommand : ICommand<OtpDeliveryResponse>
{
    public required string TargetValue { get; init; }
    public required CommunicationChannel Channel { get; init; }
    public required OtpPurpose Purpose { get; init; }
}
