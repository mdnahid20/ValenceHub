namespace ValenceHub.Api.Logging;

public sealed class FileLoggerOptions
{
    public const string SectionName = "Logging:File";

    public string Path { get; set; } = "Logs";
    public string FileNamePrefix { get; set; } = "app";
    public bool IncludeScopes { get; set; }
}
