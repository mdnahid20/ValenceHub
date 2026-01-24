
namespace ValenceHub.Application.Abstractions.Results;

/// <summary>
/// Non-generic result representing success or failure.
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error? Error { get; }

    protected Result(bool isSuccess, Error? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new Result(true, null);

    public static Result Failure(Error error)
    {
        if (error is null) throw new ArgumentNullException(nameof(error));
        return new Result(false, error);
    }

    public override string ToString() => IsSuccess ? "Success" : $"Failure: {Error}";
}

/// <summary>
/// Generic result that carries a value when successful.
/// </summary>
public sealed class Result<T> : Result
{
    private readonly T _value = default!;

    public T Value
    {
        get
        {
            if (IsFailure) throw new InvalidOperationException("Cannot access the value of a failed Result.");
            return _value;
        }
    }

    private Result(T value, bool isSuccess, Error? error) : base(isSuccess, error)
    {
        _value = value!;
    }

    public static Result<T> Success(T value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));
        return new Result<T>(value, true, null);
    }

    public static new Result<T> Failure(Error error)
    {
        if (error is null) throw new ArgumentNullException(nameof(error));
        return new Result<T>(default!, false, error);
    }
}