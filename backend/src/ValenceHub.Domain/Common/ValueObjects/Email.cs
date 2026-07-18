using System.Globalization;
using System.Text.RegularExpressions;
using ValenceHub.Domain.Abstractions;
using ValenceHub.Domain.Common.Validation;
using ValenceHub.Domain.Common.Results;

namespace ValenceHub.Domain.Common.ValueObjects;

public sealed class Email : ValueObject
{
    private const int MaxLength = 320;

    private static class EmailErrors
    {
        public static readonly Error Empty =
            Error.Validation("Email.Empty", "Email cannot be empty.");

        public static readonly Error TooLong =
            Error.Validation("Email.TooLong", $"Email cannot exceed {MaxLength} characters.");

        public static readonly Error InvalidFormat =
            Error.Validation("Email.InvalidFormat", "Invalid email format.");
    }

    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Error? TryCreate(string? email, out Email? result)
    {
        result = null;

        if (string.IsNullOrWhiteSpace(email))
            return EmailErrors.Empty;

        string normalized;
        try
        {
            normalized = NormalizeDomain(email.Trim());
        }
        catch (ArgumentException)
        {
            return EmailErrors.InvalidFormat;
        }
        catch (RegexMatchTimeoutException)
        {
            return EmailErrors.InvalidFormat;
        }

        if (normalized.Length > MaxLength)
            return EmailErrors.TooLong;

        if (!ValidationPatterns.Email.IsMatch(normalized))
            return EmailErrors.InvalidFormat;

        result = new Email(normalized.ToLowerInvariant());
        return null;
    }

    public static Error? TryCreateOptional(string? email, out Email? result)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            result = null;
            return null;
        }

        return TryCreate(email, out result);
    }

    public static Result<Email> Create(string email)
    {
        var error = TryCreateOptional(email, out var result);
        return error is null
            ? Result<Email>.Success(result!)
            : Result<Email>.Failure(error);
    }

    public static Email Restore(string value)
    {
        var error = TryCreate(value, out var email);
        if (error is not null)
            throw new InvalidOperationException(
                $"Invalid persisted email value. Code: {error.Code}, Message: {error.Message}");

        return email!;
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
