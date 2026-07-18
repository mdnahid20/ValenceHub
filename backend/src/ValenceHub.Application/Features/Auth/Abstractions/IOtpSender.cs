using ValenceHub.Domain.Common.Enums;
using ValenceHub.Domain.Otps;
using ValenceHub.Domain.Otps.Enums;

namespace ValenceHub.Application.Features.Auth.Abstractions;

public interface IOtpSender
{
    Task SendAsync(
        CommunicationChannel targetType,
        string targetValue,
        string code,
        CancellationToken cancellationToken = default);
}
