using System.Globalization;
using System.Text.RegularExpressions;
using ValenceHub.Domain.Abstractions;
using ValenceHub.Domain.Common.Results;
using ValenceHub.Domain.Common.Validation;

namespace ValenceHub.Domain.Common.ValueObjects;

public sealed class Email : ValueObject
{
    private static class EmailErrors
    {
        public static readonly Error Empty =
            Error.Validation("Email.Empty", "Email cannot be empty.");

        public static readonly Error InvalidFormat =
            Error.Validation("Email.InvalidFormat", "Invalid email format.");
    }

    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Result<Email> Create(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result<Email>.Failure(EmailErrors.Empty);

        string normalized;
        try
        {
            normalized = NormalizeDomain(email.Trim());
        }
        catch (ArgumentException)
        {
            return Result<Email>.Failure(EmailErrors.InvalidFormat);
        }
        catch (RegexMatchTimeoutException)
        {
            return Result<Email>.Failure(EmailErrors.InvalidFormat);
        }

        if (!ValidationPatterns.Email.IsMatch(normalized))
            return Result<Email>.Failure(EmailErrors.InvalidFormat);

        return Result<Email>.Success(new Email(normalized.ToLowerInvariant()));
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
