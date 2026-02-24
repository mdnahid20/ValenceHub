using ValenceHub.Domain.Abstractions;
using ValenceHub.Domain.Common.Exceptions;
using ValenceHub.Domain.Common.Validation;

namespace ValenceHub.Domain.Common.ValueObjects;

public sealed class PhoneNumber : ValueObject
{
    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static PhoneNumber Create(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new DomainException("Phone number cannot be empty.");

        var cleaned = phone.Trim().Replace(" ", "");

        if (!ValidationPatterns.Phone.IsMatch(cleaned))
            throw new DomainException("Invalid phone number format.");

        return new PhoneNumber(cleaned);
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
