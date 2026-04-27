using ValenceHub.Domain.Abstractions;
using ValenceHub.Domain.Common.Results;
using ValenceHub.Domain.Common.Validation;

namespace ValenceHub.Domain.Common.ValueObjects;

public sealed class PhoneNumber : ValueObject
{
    private const int MaxLength = 30;

    private static class PhoneNumberErrors
    {
        public static readonly Error Empty =
            Error.Validation("PhoneNumber.Empty", "Phone number cannot be empty.");

        public static readonly Error TooLong =
            Error.Validation("PhoneNumber.TooLong", $"Phone number cannot exceed {MaxLength} characters.");

        public static readonly Error InvalidFormat =
            Error.Validation("PhoneNumber.InvalidFormat", "Invalid phone number format.");
    }

    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static Error? TryCreate(string? phone, out PhoneNumber? result)
    {
        result = null;

        if (string.IsNullOrWhiteSpace(phone))
            return PhoneNumberErrors.Empty;

        var cleaned = phone.Trim().Replace(" ", "");

        if (cleaned.Length > MaxLength)
            return PhoneNumberErrors.TooLong;

        if (!ValidationPatterns.Phone.IsMatch(cleaned))
            return PhoneNumberErrors.InvalidFormat;

        result = new PhoneNumber(cleaned);
        return null;
    }

    public static Error? TryCreateOptional(string? phone, out PhoneNumber? result)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            result = null;
            return null;
        }

        return TryCreate(phone, out result);
    }

    public static Result<PhoneNumber?> Create(string? phone)
    {
        var error = TryCreateOptional(phone, out var result);
        return error is null
            ? Result<PhoneNumber?>.Success(result)
            : Result<PhoneNumber?>.Failure(error);
    }

    public static PhoneNumber Restore(string value)
    {
        var error = TryCreate(value, out var phoneNumber);
        if (error is not null)
            throw new InvalidOperationException(
                $"Invalid persisted phone number value. Code: {error.Code}, Message: {error.Message}");

        return phoneNumber!;
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
