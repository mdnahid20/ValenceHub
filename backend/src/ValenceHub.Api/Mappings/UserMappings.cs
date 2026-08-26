using ValenceHub.Api.Contracts.Users;
using ValenceHub.Application.Features.Users.Commands.CreateUser;

namespace ValenceHub.Api.Mappings;

public static class UserMappings
{
    public static CreateUserCommand ToCommand(this CreateUserRequest request)
        => new()
        {
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Password = request.Password
        };
}
