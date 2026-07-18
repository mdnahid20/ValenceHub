using ValenceHub.Application.Abstractions.Commands;

namespace ValenceHub.Application.Features.Auth.Commands.Logout;

public sealed record LogoutCommand : ICommand
{
    public required string RefreshToken { get; init; }
}
