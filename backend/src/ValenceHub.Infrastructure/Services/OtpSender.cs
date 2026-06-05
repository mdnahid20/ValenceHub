using Ardalis.GuardClauses;
using Microsoft.Extensions.DependencyInjection;
using ValenceHub.Application.Features.Auth.Abstractions;
using ValenceHub.Domain.Common.Enums;
using ValenceHub.Domain.Otps;
using ValenceHub.Domain.Otps.Enums;
using ValenceHub.Infrastructure.Attributes;

namespace ValenceHub.Infrastructure.Services;

[AutoRegister(ServiceLifetime.Scoped)]
public sealed class OtpSender : IOtpSender
{
    private readonly IEmailSender _emailSender;
    private readonly ISmsSender _smsSender;

    public OtpSender(IEmailSender emailSender, ISmsSender smsSender)
    {
        _emailSender = Guard.Against.Null(emailSender);
        _smsSender = Guard.Against.Null(smsSender);
    }

    public Task SendAsync(
        CommunicationChannel targetType,
        string targetValue,
        string code,
        CancellationToken cancellationToken = default)
        => targetType switch
        {
            CommunicationChannel.Email => _emailSender.SendOtpAsync(targetValue, code, cancellationToken),
            CommunicationChannel.SMS => _smsSender.SendOtpAsync(targetValue, code, cancellationToken),
            _ => throw new InvalidOperationException($"Unsupported OTP target type '{targetType}'.")
        };
}
