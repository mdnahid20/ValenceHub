using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ValenceHub.Infrastructure.Options;

namespace ValenceHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var emailSection = configuration.GetSection(EmailOptions.SectionName);

        services.Configure<EmailOptions>(options =>
        {
            options.SmtpHost = emailSection["SmtpHost"] ?? string.Empty;
            options.SmtpPort = int.TryParse(emailSection["SmtpPort"], out var smtpPort) ? smtpPort : 0;
            options.SmtpUsername = emailSection["SmtpUsername"] ?? string.Empty;
            options.SmtpPassword = emailSection["SmtpPassword"] ?? string.Empty;
            options.SenderEmail = emailSection["SenderEmail"] ?? string.Empty;
            options.SenderName = emailSection["SenderName"] ?? string.Empty;
            options.EnableSsl = bool.TryParse(emailSection["EnableSsl"], out var enableSsl) ? enableSsl : true;
            options.IsVirtualEmailEnabled = bool.TryParse(emailSection["IsVirtualEmailEnabled"], out var isVirtualEmailEnabled) && isVirtualEmailEnabled;
        });

        return services;
    }
}

