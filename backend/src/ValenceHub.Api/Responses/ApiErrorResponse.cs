namespace ValenceHub.Api.Responses;

public sealed record ApiErrorResponse(
    string Code,
    string Message,
    object? Metadata = null);