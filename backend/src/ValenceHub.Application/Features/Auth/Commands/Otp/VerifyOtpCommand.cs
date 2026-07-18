using ValenceHub.Application.Abstractions.Commands;
using ValenceHub.Domain.Common.Enums;
using ValenceHub.Domain.Otps.Enums;

namespace ValenceHub.Application.Features.Auth.Commands.Otp;

public sealed record VerifyOtpCommand : ICommand<VerifyOtpResponse>
{
    public required string Target { get; init; }
    public required CommunicationChannel Channel { get; init; }
    public required OtpPurpose Purpose { get; init; }
    public required string Code { get; init; }
}
