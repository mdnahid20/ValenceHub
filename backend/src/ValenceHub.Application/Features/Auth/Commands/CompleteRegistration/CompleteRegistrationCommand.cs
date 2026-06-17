using ValenceHub.Application.Abstractions.Commands;
using ValenceHub.Domain.Otps.Enums;

namespace ValenceHub.Application.Features.Auth.Commands.CompleteRegistration;

public sealed record CompleteRegistrationCommand : ICommand
{
    public required Guid UserId { get; init; }
    public required string Token { get; init; }
}
