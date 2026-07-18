using ValenceHub.Application.Abstractions.Commands;

namespace ValenceHub.Application.Features.Auth.Commands.ChangePassword;

public sealed record ChangePasswordCommand : ICommand
{
    public required string CurrentPassword { get; init; }
    public required string NewPassword { get; init; }
}
