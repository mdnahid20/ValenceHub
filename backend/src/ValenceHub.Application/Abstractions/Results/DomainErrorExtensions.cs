namespace ValenceHub.Application.Abstractions.Results;

public static class DomainErrorExtensions
{
    /// <summary>
    /// Convert a domain Error to an application Result.
    /// </summary>
    public static Result<T> ToResult<T>(this ValenceHub.Domain.Common.Results.Error error)
    {
        if (error is null) throw new ArgumentNullException(nameof(error));
        return Result<T>.Failure(new Error(error.Code, error.Message, ErrorType.Validation));
    }
}
