using ValenceHub.Domain.Common.Results;

namespace ValenceHub.Domain.Otps;

public static class OtpCodeErrors
{
    public static readonly Error InvalidCode =
        Error.Validation("Otp.Code.Invalid", "The OTP code is invalid.");

    public static readonly Error CodeAlreadyUsed =
        Error.Validation("Otp.Code.Used", "The OTP code has already been used.");

    public static readonly Error CodeExpired =
        Error.Validation("Otp.Code.Expired", "The OTP code has expired.");

    public static readonly Error VerificationTokenInvalid =
        Error.Validation("Otp.VerificationToken.Invalid", "The verification token is invalid.");

    public static readonly Error VerificationTokenExpired =
        Error.Validation("Otp.VerificationToken.Expired", "The verification token has expired.");

    public static readonly Error VerificationTokenAlreadyConsumed =
        Error.Validation("Otp.VerificationToken.Consumed", "The verification token has already been consumed.");

    public static readonly Error CodeIsNotUsed =
        Error.Validation("Otp.Code.NotUsed", "The OTP code has not been used yet.");

    public static readonly Error TargetValueRequired =
        Error.Validation("Otp.TargetValue.Required", "The OTP target value is required.");

    public static Error AttemptsExceeded(int maxAttempts) =>
        Error.Validation(
            "Otp.Attempts.Exceeded",
            $"The OTP code has exceeded the maximum of {maxAttempts} attempts.");

    public static Error ResendLimitExceeded(int maxResends) =>
        Error.Validation(
            "Otp.Resend.Exceeded",
            $"The OTP code has exceeded the maximum of {maxResends} resends.");

    public static Error ResendNotAllowedUntil(DateTimeOffset retryAtUtc) =>
        Error.Validation(
            "Otp.Resend.TooSoon",
            $"OTP resend is not allowed until {retryAtUtc:O}.");

    public static readonly Error CodeHashRequired =
        Error.Validation("Otp.CodeHash.Required", "OTP code hash is required.");

    public static readonly Error PurposeRequired =
        Error.Validation("Otp.Purpose.Required", "OTP purpose is required.");

    public static readonly Error TargetRequired =
        Error.Validation("Otp.Target.Required", "OTP target is required.");
}
