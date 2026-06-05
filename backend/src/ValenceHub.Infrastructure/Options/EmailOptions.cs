namespace ValenceHub.Infrastructure.Options;

public sealed class EmailOptions
{
    public const string SectionName = "Email";
    public required string SmtpHost { get; set; }
    public required int SmtpPort { get; set; }
    public required string SmtpUsername { get; set; }
    public required string SmtpPassword { get; set; }
    public required string SenderEmail { get; set; }
    public required string SenderName { get; set; }
    public bool EnableSsl { get; set; } = true;
    public bool IsVirtualEmailEnabled { get; set; }
}
