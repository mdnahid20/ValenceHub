using ValenceHub.Domain.Abstractions;
using ValenceHub.Domain.Common.Results;
using ValenceHub.Domain.Common.Validation;

namespace ValenceHub.Domain.Common.ValueObjects;

public sealed class PhoneNumber : ValueObject
{
    private static class PhoneNumberErrors
    {
        public static readonly Error Empty =
            Error.Validation("PhoneNumber.Empty", "Phone number cannot be empty.");

        public static readonly Error InvalidFormat =
            Error.Validation("PhoneNumber.InvalidFormat", "Invalid phone number format.");
    }

    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static Result<PhoneNumber> Create(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return Result<PhoneNumber>.Failure(PhoneNumberErrors.Empty);

        var cleaned = phone.Trim().Replace(" ", "");

        if (!ValidationPatterns.Phone.IsMatch(cleaned))
            return Result<PhoneNumber>.Failure(PhoneNumberErrors.InvalidFormat);

        return Result<PhoneNumber>.Success(new PhoneNumber(cleaned));
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
