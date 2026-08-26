using ValenceHub.Api.Contracts.Auth;
using ValenceHub.Application.Features.Auth.Commands.ChangePassword;
using ValenceHub.Application.Features.Auth.Commands.CompleteRegistration;
using ValenceHub.Application.Features.Auth.Commands.ForgotPassword;
using ValenceHub.Application.Features.Auth.Commands.Login;
using ValenceHub.Application.Features.Auth.Commands.Logout;
using ValenceHub.Application.Features.Auth.Commands.Otp;
using ValenceHub.Application.Features.Auth.Commands.RefreshToken;
using ValenceHub.Application.Features.Users.Commands.CreateUser;
using ValenceHub.Domain.Common.Enums;
using ValenceHub.Domain.Otps.Enums;

namespace ValenceHub.Api.Mappings;

public static class AuthMappings
{
    public static LoginCommand ToCommand(this LoginRequest request)
        => new()
        {
            LoginId = request.LoginId,
            Password = request.Password
        };

    public static CreateUserCommand ToCommand(this RegisterRequest request)
        => new()
        {
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Password = request.Password
        };

    public static RefreshTokenCommand ToCommand(this RefreshTokenRequest request)
        => new() { RefreshToken = request.RefreshToken };

    public static LogoutCommand ToCommand(this LogoutRequest request)
        => new() { RefreshToken = request.RefreshToken };

    public static ChangePasswordCommand ToCommand(this ChangePasswordRequest request)
        => new()
        {
            CurrentPassword = request.CurrentPassword,
            NewPassword = request.NewPassword
        };

    public static SendOtpCommand ToCommand(this SendOtpRequest request)
    {
        return new()
        {
            Target = request.Target,
            Channel = (CommunicationChannel)request.Channel,
            Purpose = (OtpPurpose)request.Purpose
        };
    }

    public static ResendOtpCommand ToCommand(this ResendOtpRequest request)
        => new()
        {
            TargetValue = request.TargetValue,
            Channel = (CommunicationChannel)request.Channel,
            Purpose = (OtpPurpose)request.Purpose
        };

    public static VerifyOtpCommand ToCommand(this VerifyOtpRequest request)
        => new()
        {
            Target = request.Target,
            Channel = (CommunicationChannel)request.Channel,
            Purpose = (OtpPurpose)request.Purpose,
            Code = request.Code
        };

    public static CompleteRegistrationCommand ToCommand(this CompleteRegistrationRequest request)
        => new()
        {
            UserId = request.UserId,
            Token = request.Token
        };

    public static ForgotPasswordCommand ToCommand(this ForgotPasswordRequest request)
        => new()
        {
            UserId = request.UserId,
            VerificationToken = request.VerificationToken,
            NewPassword = request.NewPassword
        };
}
