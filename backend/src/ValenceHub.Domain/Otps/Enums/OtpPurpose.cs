using ValenceHub.Domain.Abstractions;
using ValenceHub.Domain.Common.Results;

namespace ValenceHub.Domain.Otps.Enums;

public enum OtpPurpose 
{
    Register = 1,

    Login = 2,  

    ResetPassword = 3,

    Logout = 4,

    EmailChange = 5,

    PhoneChange = 6,

    PasswordChange = 7, 

    TwoFactorAuth = 8,
}
