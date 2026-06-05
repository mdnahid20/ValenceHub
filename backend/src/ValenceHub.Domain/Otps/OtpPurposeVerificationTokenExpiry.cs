using ValenceHub.Domain.Otps.Enums;

namespace ValenceHub.Domain.Otps;

public static class OtpPurposeVerificationTokenExpiry
{
    public static TimeSpan GetDuration(
        OtpPurpose purpose)
    {
        return purpose switch
        {
            OtpPurpose.Register =>
                TimeSpan.FromMinutes(15),

            OtpPurpose.Login =>
                TimeSpan.FromMinutes(10),

            OtpPurpose.ResetPassword =>
                TimeSpan.FromMinutes(10),

            OtpPurpose.Logout =>
                TimeSpan.FromMinutes(5),

            OtpPurpose.EmailChange =>
                TimeSpan.FromMinutes(15),

            OtpPurpose.PhoneChange =>
                TimeSpan.FromMinutes(15),

            OtpPurpose.PasswordChange =>
                TimeSpan.FromMinutes(10),

            OtpPurpose.TwoFactorAuth =>
                TimeSpan.FromMinutes(5),

            _ =>
                TimeSpan.FromMinutes(10)
        };
    }
}