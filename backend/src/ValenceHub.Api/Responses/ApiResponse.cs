namespace ValenceHub.Api.Responses;

public sealed record ApiResponse<T>(bool Success,T? Data, ApiErrorResponse? Error, string TraceId, DateTimeOffset TimestampUtc)
{
    public static ApiResponse<T> Ok(T data, string traceId, DateTimeOffset timestampUtc)
        => new(true, data, null, traceId, timestampUtc);

    public static ApiResponse<T> Fail(
        ApiErrorResponse error,
        string traceId,
        DateTimeOffset timestampUtc)
        => new(false, default, error, traceId, timestampUtc);
}
