using ValenceHub.Api.Contracts.Auth;
using ValenceHub.Application.Features.Auth.Commands.Login;
using ValenceHub.Application.Features.Auth.Commands.Logout;

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
}
