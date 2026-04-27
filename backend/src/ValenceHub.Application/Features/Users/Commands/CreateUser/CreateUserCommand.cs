using ValenceHub.Application.Abstractions.Commands;

namespace ValenceHub.Application.Features.Users.Commands.CreateUser;

public sealed record CreateUserCommand : ICommand<Guid>
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required string PhoneNumber { get; init; }
    public required string Password { get; init; }
}
