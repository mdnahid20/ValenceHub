using System;
using System.Collections.Generic;
using System.Text;
using ValenceHub.Domain.Otps.Enums;

namespace ValenceHub.Domain.Otps;

public static class OtpPurposeExpiry
{
    public static TimeSpan GetDuration(
        OtpPurpose purpose)
    {
        return purpose switch
        {
            OtpPurpose.Register =>
                TimeSpan.FromSeconds(60),

            OtpPurpose.Login =>
                TimeSpan.FromSeconds(60),

            OtpPurpose.ForgotPassword =>
                TimeSpan.FromSeconds(60),

            OtpPurpose.Logout =>
                TimeSpan.FromSeconds(60),

            OtpPurpose.EmailChange =>
                TimeSpan.FromSeconds(60),

            OtpPurpose.PhoneChange =>
                TimeSpan.FromSeconds(60),

            OtpPurpose.PasswordChange =>
                TimeSpan.FromSeconds(60),

            OtpPurpose.TwoFactorAuth =>
                TimeSpan.FromSeconds(60),

            _ =>
                TimeSpan.FromSeconds(60)
        };
    }
}
