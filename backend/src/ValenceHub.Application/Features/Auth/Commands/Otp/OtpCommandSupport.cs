using ValenceHub.Domain.Common.Enums;
using ValenceHub.Domain.Common.ValueObjects;
using ValenceHub.Domain.Otps.Enums;

namespace ValenceHub.Application.Features.Auth.Commands.Otp;

internal static class OtpCommandSupport
{
    public static bool TryParsePublicPurpose(string rawPurpose, out OtpPurpose purpose)
    {
        var isParsed = Enum.TryParse(rawPurpose?.Trim(), ignoreCase: true, out purpose);
        return isParsed && IsSupportedPublicPurpose(purpose);
    }

    public static bool IsSupportedTarget(string rawTarget)
        => TryResolveTarget(rawTarget, out _, out _);

    public static bool TryResolveTarget(
        string rawTarget,
        out CommunicationChannel targetType,
        out string normalizedTarget)
    {
        var isResolved = TryResolveTarget(rawTarget, out targetType, out normalizedTarget, out _);
        return isResolved;
    }

    public static bool TryResolveTarget(
        string rawTarget,
        out CommunicationChannel targetType,
        out string normalizedTarget,
        out string maskedTarget)
    {
        var email = Email.Create(rawTarget);
        if (email.IsSuccess)
        {
            targetType = CommunicationChannel.Email;
            normalizedTarget = email.Value.Value;
            maskedTarget = MaskEmail(email.Value.Value);
            return true;
        }

        var phoneNumber = PhoneNumber.Create(rawTarget);
        if (phoneNumber.IsSuccess)
        {
            targetType = CommunicationChannel.SMS;
            normalizedTarget = phoneNumber.Value.Value;
            maskedTarget = MaskPhone(phoneNumber.Value.Value);
            return true;
        }

        targetType = default;
        normalizedTarget = string.Empty;
        maskedTarget = string.Empty;
        return false;
    }

    public static bool IsSupportedPublicPurpose(OtpPurpose purpose)
        => purpose is OtpPurpose.Register or OtpPurpose.Login or OtpPurpose.ForgotPassword;

    public static string ToPurposeValue(OtpPurpose purpose)
        => purpose.ToString();

    public static string ToMaskedValue(CommunicationChannel targetType, string targetValue)
        => targetType switch
        {
            CommunicationChannel.Email => MaskEmail(targetValue),
            CommunicationChannel.SMS => MaskPhone(targetValue),
            _ => "***"
        };

    private static string MaskEmail(string email)
    {
        var atIndex = email.IndexOf('@');
        if (atIndex <= 0)
            return "***";

        var local = email[..atIndex];
        var domain = email[atIndex..];
        var visibleLength = Math.Min(2, local.Length);
        var visible = local[..visibleLength];
        var hiddenCount = Math.Max(1, local.Length - visibleLength);

        return $"{visible}{new string('*', hiddenCount)}{domain}";
    }

    private static string MaskPhone(string phoneNumber)
    {
        var prefix = phoneNumber.StartsWith('+') ? "+" : string.Empty;
        var value = prefix.Length == 0 ? phoneNumber : phoneNumber[1..];
        var visibleCount = Math.Min(4, value.Length);
        var hiddenCount = Math.Max(2, value.Length - visibleCount);
        var suffix = value[^visibleCount..];

        return $"{prefix}{new string('*', hiddenCount)}{suffix}";
    }
}
