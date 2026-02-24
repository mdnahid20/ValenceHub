using System.Globalization;
using System.Text.RegularExpressions;
using ValenceHub.Domain.Abstractions;
using ValenceHub.Domain.Common.Exceptions;
using ValenceHub.Domain.Common.Validation;

namespace ValenceHub.Domain.Common.ValueObjects;

public sealed class Email : ValueObject
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty.");

        var normalized = NormalizeDomain(email.Trim());

        if (!ValidationPatterns.Email.IsMatch(normalized))
            throw new DomainException("Invalid email format.");

        return new Email(normalized.ToLowerInvariant());
    }

    private static string NormalizeDomain(string email)
    {
        return Regex.Replace(
            email,
            @"(@)(.+)$",
            match =>
            {
                var idn = new IdnMapping();
                var domainName = idn.GetAscii(match.Groups[2].Value);
                return match.Groups[1].Value + domainName;
            },
            RegexOptions.None,
            TimeSpan.FromMilliseconds(ValidationPatterns.RegexTimeoutMilliseconds));
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
