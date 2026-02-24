using System.Text.RegularExpressions;

namespace ValenceHub.Domain.Common.Validation;

public static class ValidationPatterns
{
    public const int RegexTimeoutMilliseconds = 200;

    // Simple but safe email format validation
    public static readonly Regex Email =
        new(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase,
            TimeSpan.FromMilliseconds(RegexTimeoutMilliseconds));

    // E.164 international phone number format
    // Example: +8801712345678
    public static readonly Regex Phone =
        new(
            @"^\+?[1-9]\d{7,14}$",
            RegexOptions.Compiled,
            TimeSpan.FromMilliseconds(RegexTimeoutMilliseconds));
}
