using ValenceHub.Application.Abstractions.Commands;

namespace ValenceHub.Application.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand : ICommand<RefreshTokenResponse>
{
    public required string RefreshToken { get; init; }
}
