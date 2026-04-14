using System;
using ValenceHub.Application.Abstractions.Results;

namespace ValenceHub.Application.Common
{
    public static class Guard
    {
        public static Result NotNull(object? value, string name)
        {
            return value is null
                ? Result.Failure(new Error(name, $"{name} must not be null."))
                : Result.Success();
        }

        public static Result<T> NotNull<T>(T? value, string name) where T : class
        {
            return value is null
                ? Result<T>.Failure(new Error(name, $"{name} must not be null."))
                : Result<T>.Success(value);
        }

        public static Result NotNullOrEmpty(string? value, string name)
        {
            return string.IsNullOrWhiteSpace(value)
                ? Result.Failure(new Error(name, $"{name} must not be empty."))
                : Result.Success();
        }

        public static Result<string> NotNullOrEmptyAsResult(string? value, string name)
        {
            return string.IsNullOrWhiteSpace(value)
                ? Result<string>.Failure(new Error(name, $"{name} must not be empty."))
                : Result<string>.Success(value!);
        }

        public static Result NotEmpty(Guid id, string name)
        {
            return id == Guid.Empty
                ? Result.Failure(new Error(name, $"{name} must not be an empty GUID."))
                : Result.Success();
        }

        public static Result<T> Ensure<T>(T value, Func<T, bool> predicate, string errorMessage)
        {
            return predicate(value)
                ? Result<T>.Failure(new Error(string.Empty, errorMessage))
                : Result<T>.Success(value);
        }

        public static Result Against(bool predicate, string errorMessage)
        {
            return predicate
                ? Result.Failure(new Error(string.Empty, errorMessage))
                : Result.Success();
        }
    }
}
