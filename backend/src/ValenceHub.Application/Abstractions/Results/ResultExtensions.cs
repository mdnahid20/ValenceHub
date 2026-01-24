
namespace ValenceHub.Application.Abstractions.Results;

/// <summary>
/// Small helpers to create or transform Result instances.
/// </summary>
public static class ResultExtensions
{
    public static Result ToResult(this bool condition, Error onFailure)
        => condition ? Result.Success() : Result.Failure(onFailure);

    public static Result<T> ToResult<T>(this T? value, Error onNull)
        => value is null ? Result<T>.Failure(onNull) : Result<T>.Success(value);

    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> map)
    {
        if (result is null) throw new ArgumentNullException(nameof(result));
        if (result.IsFailure) return Result<TOut>.Failure(result.Error!);
        return Result<TOut>.Success(map(result.Value));
    }
}