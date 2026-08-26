
namespace ValenceHub.Application.Abstractions.Results;

/// <summary>
/// Non-generic result representing success or failure.
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new ArgumentException("Success result cannot have an error.");

        if (!isSuccess && error == Error.None)
            throw new ArgumentException("Failure result must have an error.");

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, Error.None);

    public static Result Failure(Error error)
    {
        if (error is null) throw new ArgumentNullException(nameof(error));
        return new(false, error);
    }

    public override string ToString() => IsSuccess ? "Success" : $"Failure: {Error}";
}

/// <summary>
/// Generic result that carries a value when successful.
/// </summary>
public sealed class Result<T> : Result
{
    public T? Value { get; }

    private Result(T value) : base(true, Error.None)
        => Value = value;

    private Result(Error error) : base(false, error)
        => Value = default;

    public static Result<T> Success(T value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));
        return new(value);
    }

    public static new Result<T> Failure(Error error)
    {
        if (error is null) throw new ArgumentNullException(nameof(error));
        return new(error);
    }

    /// <summary>
    /// Implicit conversion from T to Result<T>.Success(value)
    /// </summary>
    public static implicit operator Result<T>(T value)
        => Success(value);
}