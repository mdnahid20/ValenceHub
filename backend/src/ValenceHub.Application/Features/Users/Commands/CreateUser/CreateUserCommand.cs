using ValenceHub.Application.Abstractions.Commands;

namespace ValenceHub.Application.Features.Users.Commands.CreateUser;

public sealed record CreateUserCommand : ICommand<Guid>
{
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public required string Password { get; init; }
}
