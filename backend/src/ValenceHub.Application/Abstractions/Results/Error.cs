
namespace ValenceHub.Application.Abstractions.Results;

public sealed record Error(
    string Code,
    string Message,
    ErrorType Type = ErrorType.Failure,
    IReadOnlyDictionary<string, object>? Metadata = null)
{
    public static readonly Error None = new("", "");

    public Error(string message) : this(string.Empty, message) { }

    public static Error Failure(string code, string message)
        => new(code, message, ErrorType.Failure);

    public static Error Validation(string code, string message)
        => new(code, message, ErrorType.Validation);

    public static Error NotFound(string code, string message)
        => new(code, message, ErrorType.NotFound);

    public static Error Conflict(string code, string message)
        => new(code, message, ErrorType.Conflict);

    public static Error Unauthorized(string code, string message)
        => new(code, message, ErrorType.Unauthorized);

    public static Error Forbidden(string code, string message)
        => new(code, message, ErrorType.Forbidden);
    public static Error NotSupported(string code, string message)
    => new Error(code, message, ErrorType.NotSupported);

    public override string ToString() => string.IsNullOrEmpty(Code) ? Message : $"{Code}: {Message}";
}

public enum ErrorType
{
    Failure,
    Validation,
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden,
    NotSupported
}