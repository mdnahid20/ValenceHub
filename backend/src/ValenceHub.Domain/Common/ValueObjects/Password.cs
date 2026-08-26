using ValenceHub.Domain.Abstractions;
using ValenceHub.Domain.Common.Results;

namespace ValenceHub.Domain.Common.ValueObjects;

public sealed class Password : ValueObject
{
    private const int MinLength = 8;
    private const int MaxLength = 255;

    private static class PasswordErrors
    {
        public static readonly Error Empty =
            Error.Validation("Password.Empty", "Password cannot be empty.");

        public static readonly Error TooShort =
            Error.Validation("Password.TooShort", $"Password must be at least {MinLength} characters.");

        public static readonly Error TooLong =
            Error.Validation("Password.TooLong", $"Password cannot exceed {MaxLength} characters.");

        public static readonly Error NoUppercase =
            Error.Validation("Password.NoUppercase", "Password must contain at least one uppercase letter.");

        public static readonly Error NoLowercase =
            Error.Validation("Password.NoLowercase", "Password must contain at least one lowercase letter.");

        public static readonly Error NoDigit =
            Error.Validation("Password.NoDigit", "Password must contain at least one digit.");

        public static readonly Error NoSpecialChar =
            Error.Validation("Password.NoSpecialChar", "Password must contain at least one special character (!@#$%^&*).");
    }

    public string Value { get; }

    private Password(string value)
    {
        Value = value;
    }

    public static Error? TryCreate(string? password, out Password? result)
    {
        result = null;

        if (string.IsNullOrWhiteSpace(password))
            return PasswordErrors.Empty;

        password = password.Trim();

        if (password.Length < MinLength)
            return PasswordErrors.TooShort;

        if (password.Length > MaxLength)
            return PasswordErrors.TooLong;

        if (!password.Any(char.IsUpper))
            return PasswordErrors.NoUppercase;

        if (!password.Any(char.IsLower))
            return PasswordErrors.NoLowercase;

        if (!password.Any(char.IsDigit))
            return PasswordErrors.NoDigit;

        if (!password.Any(c => "!@#$%^&*".Contains(c)))
            return PasswordErrors.NoSpecialChar;

        result = new Password(password);
        return null;
    }

    public static Result<Password> Create(string? password)
    {
        var error = TryCreate(password, out var result);
        return error is null
            ? Result<Password>.Success(result!)
            : Result<Password>.Failure(error);
    }

    public static Password Restore(string value)
    {
        var error = TryCreate(value, out var password);
        if (error is not null)
            throw new InvalidOperationException(
                $"Invalid persisted password value. Code: {error.Code}, Message: {error.Message}");

        return password!;
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => "****";
}
