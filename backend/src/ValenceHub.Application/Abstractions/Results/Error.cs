
namespace ValenceHub.Application.Abstractions.Results;

public sealed record Error(string Code, string Message)
{
    public Error(string message) : this(string.Empty, message) { }
    public override string ToString() => string.IsNullOrEmpty(Code) ? Message : $"{Code}: {Message}";
}