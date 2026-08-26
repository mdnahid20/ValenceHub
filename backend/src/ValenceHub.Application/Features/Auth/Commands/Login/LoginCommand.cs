using ValenceHub.Application.Abstractions.Commands;

namespace ValenceHub.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand : ICommand<LoginResponse>
{
    public required string LoginId { get; init; }
    public required string Password { get; init; }
}
