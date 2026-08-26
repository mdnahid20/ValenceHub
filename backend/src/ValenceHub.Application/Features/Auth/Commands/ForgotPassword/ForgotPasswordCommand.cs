using ValenceHub.Application.Abstractions.Commands;

namespace ValenceHub.Application.Features.Auth.Commands.ForgotPassword;

public sealed record ForgotPasswordCommand : ICommand
{
    public required Guid UserId { get; init; }
    public required string VerificationToken { get; init; }
    public required string NewPassword { get; init; }
}
