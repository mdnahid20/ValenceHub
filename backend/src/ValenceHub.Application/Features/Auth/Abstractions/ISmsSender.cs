using ValenceHub.Domain.Otps.Enums;

namespace ValenceHub.Application.Features.Auth.Abstractions;

public interface ISmsSender
{
    Task SendOtpAsync(
        string phoneNumber,
        string code,
        CancellationToken cancellationToken = default);
}
