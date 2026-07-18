using ValenceHub.Domain.Otps.Enums;

namespace ValenceHub.Application.Features.Auth.Abstractions;

public interface IEmailSender
{
    Task SendOtpAsync(
        string email,
        string code,
        CancellationToken cancellationToken = default);
}
