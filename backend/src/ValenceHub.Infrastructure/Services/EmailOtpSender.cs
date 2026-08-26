using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ValenceHub.Application.Features.Auth.Abstractions;
using ValenceHub.Domain.Otps.Enums;
using ValenceHub.Infrastructure.Attributes;
using ValenceHub.Infrastructure.Options;

namespace ValenceHub.Infrastructure.Services;

[AutoRegister(ServiceLifetime.Scoped)]
public sealed class EmailOtpSender : IEmailSender
{
    private readonly ILogger<EmailOtpSender> _logger;
    private readonly EmailOptions _emailOptions;

    public EmailOtpSender(
        ILogger<EmailOtpSender> logger,
        IOptions<EmailOptions> emailOptions)
    {
        _logger = logger;
        _emailOptions = emailOptions.Value;
    }

    public async Task SendOtpAsync(
        string email,
        string code,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Sending OTP email. Target: {Email}, Code: {Code}.",
                email,
                code);

            if (_emailOptions.IsVirtualEmailEnabled)
            {
                _logger.LogInformation(
                    "Virtual email mode enabled. Email would be sent to {Email} with OTP: {Code}",
                    email,
                    code);
                return;
            }

            using var smtpClient = new SmtpClient(_emailOptions.SmtpHost, _emailOptions.SmtpPort)
            {
                Credentials = new NetworkCredential(_emailOptions.SmtpUsername, _emailOptions.SmtpPassword),
                EnableSsl = _emailOptions.EnableSsl,
                Timeout = 10000
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailOptions.SenderEmail, _emailOptions.SenderName),
                Subject = "Your ValenceHub OTP Code",
                Body = GenerateEmailBody(code),
                IsBodyHtml = true
            };

            mailMessage.To.Add(new MailAddress(email));

            await smtpClient.SendMailAsync(mailMessage, cancellationToken);

            _logger.LogInformation(
                "OTP email successfully sent to {Email}.",
                email);
        }
        catch (SmtpException smtpEx)
        {
            _logger.LogError(
                smtpEx,
                "SMTP error while sending OTP email to {Email}. Status: {StatusCode}",
                email,
                smtpEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while sending OTP email to {Email}.",
                email);
            throw;
        }
    }

    private static string GenerateEmailBody(string code)
    {
        return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Your OTP Code</title>
    <style>
        body {{
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
            background-color: #f5f5f5;
            margin: 0;
            padding: 0;
        }}
        .container {{
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
            padding: 40px;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }}
        .header {{
            text-align: center;
            margin-bottom: 30px;
        }}
        .logo {{
            font-size: 24px;
            font-weight: bold;
            color: #333;
        }}
        .content {{
            text-align: center;
            margin-bottom: 30px;
        }}
        .message {{
            font-size: 16px;
            color: #666;
            margin-bottom: 20px;
            line-height: 1.6;
        }}
        .otp-box {{
            background-color: #f0f0f0;
            border: 2px solid #ddd;
            border-radius: 4px;
            padding: 20px;
            margin: 20px 0;
        }}
        .otp-code {{
            font-size: 32px;
            font-weight: bold;
            color: #333;
            letter-spacing: 5px;
            font-family: 'Courier New', monospace;
        }}
        .otp-expiry {{
            font-size: 12px;
            color: #999;
            margin-top: 10px;
        }}
        .footer {{
            text-align: center;
            font-size: 12px;
            color: #999;
            border-top: 1px solid #eee;
            padding-top: 20px;
            margin-top: 30px;
        }}
        .warning {{
            background-color: #fff3cd;
            border: 1px solid #ffc107;
            color: #856404;
            padding: 10px;
            border-radius: 4px;
            font-size: 13px;
            margin-top: 20px;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <div class=""logo"">ValenceHub</div>
        </div>
        <div class=""content"">
            <div class=""message"">
                Your One-Time Password (OTP) for secure authentication:
            </div>
            <div class=""otp-box"">
                <div class=""otp-code"">{code}</div>
                <div class=""otp-expiry"">Valid for 10 minutes</div>
            </div>
            <div class=""message"">
                This code is for your security. Never share it with anyone.
            </div>
            <div class=""warning"">
                If you didn't request this code, please ignore this email and your account will remain secure.
            </div>
        </div>
        <div class=""footer""> /* TODO : Replace Time & Use System Time */
            <p>© {DateTime.UtcNow.Year} ValenceHub. All rights reserved.</p>
            <p>This is an automated email. Please do not reply to this message.</p>
        </div>
    </div>
</body>
</html>";
    }
}
