using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ValenceHub.Application.Features.Auth.Abstractions;
using ValenceHub.Domain.Otps.Enums;
using ValenceHub.Infrastructure.Attributes;

namespace ValenceHub.Infrastructure.Services;

[AutoRegister(ServiceLifetime.Scoped)]
public sealed class SmsOtpSender : ISmsSender
{
    private readonly ILogger<SmsOtpSender> _logger;

    public SmsOtpSender(ILogger<SmsOtpSender> logger)
    {
        _logger = logger;
    }

    public Task SendOtpAsync(
        string phoneNumber,
        string code,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Simulated SMS OTP dispatch. Target: {PhoneNumber}, Code: {Code}.",
            phoneNumber,
            code);

        return Task.CompletedTask;
    }
}
