using ValenceHub.Api.Contracts.Auth;
using ValenceHub.Application.Features.Auth.Commands.Login;

namespace ValenceHub.Api.Mappings;

public static class AuthMappings
{
    public static LoginCommand ToCommand(this LoginRequest request)
        => new()
        {
            LoginId = request.LoginId,
            Password = request.Password
        };
    public static RefreshTokenCommand ToCommand(this RefreshTokenRequest request)
        => new() { RefreshToken = request.RefreshToken };
}
