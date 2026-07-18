
namespace ValenceHub.Application.Abstractions.Results;

/// <summary>
/// Functional helpers to create or transform Result instances.
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
        if (result.IsFailure) return Result<TOut>.Failure(result.Error);
        return Result<TOut>.Success(map(result.Value!));
    }

    /// <summary>
    /// Monadic bind for chaining operations that return Results.
    /// </summary>
    public static async Task<Result<TOut>> Bind<TIn, TOut>(
        this Task<Result<TIn>> resultTask,
        Func<TIn, Task<Result<TOut>>> func)
    {
        if (resultTask is null) throw new ArgumentNullException(nameof(resultTask));
        if (func is null) throw new ArgumentNullException(nameof(func));

        var result = await resultTask;

        if (result.IsFailure)
            return Result<TOut>.Failure(result.Error);

        return await func(result.Value!);
    }

    /// <summary>
    /// Convert an Error to a failed Result.
    /// </summary>
    public static Result<T> ToResult<T>(this Error error)
    {
        if (error is null) throw new ArgumentNullException(nameof(error));
        return Result<T>.Failure(error);
    }
}