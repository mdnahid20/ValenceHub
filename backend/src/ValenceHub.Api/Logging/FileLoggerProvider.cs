using System.Text;
using Microsoft.Extensions.Logging;
using ValenceHub.Application.Abstractions.Services;

namespace ValenceHub.Api.Logging;

public sealed class FileLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    private readonly string _contentRootPath;
    private readonly FileLoggerOptions _options;
    private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;
    private readonly object _sync = new();
    private IExternalScopeProvider _scopeProvider = new LoggerExternalScopeProvider();

    public FileLoggerProvider(
        FileLoggerOptions options,
        string contentRootPath,
        IDateTimeOffsetProvider dateTimeOffsetProvider)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _contentRootPath = contentRootPath ?? throw new ArgumentNullException(nameof(contentRootPath));
        _dateTimeOffsetProvider = dateTimeOffsetProvider ?? throw new ArgumentNullException(nameof(dateTimeOffsetProvider));
    }

    public ILogger CreateLogger(string categoryName) => new FileLogger(categoryName, this);

    public void Dispose()
    {
    }

    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        _scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
    }

    internal IDisposable? PushScope<TState>(TState state) where TState : notnull
        => _scopeProvider.Push(state);

    internal bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    internal void WriteLog(
        string categoryName,
        LogLevel logLevel,
        EventId eventId,
        string message,
        Exception? exception)
    {
        var now = _dateTimeOffsetProvider.UtcNow;
        var logDirectory = ResolveLogDirectory(now);
        var logFilePath = Path.Combine(
            logDirectory,
            BuildLogFileName(now));

        var entryBuilder = new StringBuilder();
        entryBuilder
            .Append(now.ToString("yyyy-MM-dd HH:mm:ss.fff zzz"))
            .Append(" [")
            .Append(GetLogLevelCode(logLevel))
            .Append("] ")
            .Append(categoryName);

        if (eventId.Id != 0 || !string.IsNullOrWhiteSpace(eventId.Name))
        {
            entryBuilder
                .Append(" (EventId: ")
                .Append(eventId.Id);

            if (!string.IsNullOrWhiteSpace(eventId.Name))
            {
                entryBuilder
                    .Append(", ")
                    .Append(eventId.Name);
            }

            entryBuilder.Append(')');
        }

        if (!string.IsNullOrWhiteSpace(message))
        {
            entryBuilder
                .Append(": ")
                .Append(message);
        }

        entryBuilder.AppendLine();

        if (_options.IncludeScopes)
        {
            var scopes = new List<string>();
            _scopeProvider.ForEachScope(
                (scope, state) =>
                {
                    var text = scope?.ToString();
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        state.Add(text);
                    }
                },
                scopes);

            if (scopes.Count > 0)
            {
                entryBuilder
                    .Append("Scopes: ")
                    .AppendJoin(" => ", scopes)
                    .AppendLine();
            }
        }

        if (exception is not null)
        {
            entryBuilder.AppendLine(exception.ToString());
        }

        lock (_sync)
        {
            Directory.CreateDirectory(logDirectory);
            File.AppendAllText(logFilePath, entryBuilder.ToString(), Encoding.UTF8);
        }
    }

    private string ResolveLogDirectory(DateTimeOffset now)
    {
        var rootPath = Path.IsPathRooted(_options.Path)
            ? _options.Path
            : Path.Combine(_contentRootPath, _options.Path);

        return Path.Combine(rootPath, now.ToString("yyyy"), now.ToString("MM"));
    }

    private string BuildLogFileName(DateTimeOffset now)
    {
        var configuredName = string.IsNullOrWhiteSpace(_options.FileNamePrefix)
            ? "app"
            : _options.FileNamePrefix.Trim();

        var normalizedName = configuredName.Trim('-', '_', '.', ' ');
        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            normalizedName = "app";
        }

        return $"{now:yyyyMMdd}-{normalizedName}.txt";
    }

    private static string GetLogLevelCode(LogLevel logLevel) => logLevel switch
    {
        LogLevel.Trace => "TRC",
        LogLevel.Debug => "DBG",
        LogLevel.Information => "INF",
        LogLevel.Warning => "WRN",
        LogLevel.Error => "ERR",
        LogLevel.Critical => "CRT",
        _ => logLevel.ToString().ToUpperInvariant()
    };
}
