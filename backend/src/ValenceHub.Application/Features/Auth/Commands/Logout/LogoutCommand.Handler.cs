using MediatR;
using ValenceHub.Application.Abstractions.Commands;
using ValenceHub.Application.Abstractions.Results;
using ValenceHub.Application.Abstractions.Services;
using ValenceHub.Application.Features.Auth.Abstractions;

namespace ValenceHub.Application.Features.Auth.Commands.Logout;

public sealed class LogoutCommandHandler : ICommandHandler<LogoutCommand>
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IDateTimeOffsetProvider _clock;

    public LogoutCommandHandler(
        IRefreshTokenService refreshTokenService,
        IDateTimeOffsetProvider clock)
    {
        _refreshTokenService = refreshTokenService;
        _clock = clock;
    }

    public async Task<Result<Unit>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var revokeResult = await _refreshTokenService.RevokeAsync(
            request.RefreshToken,
            _clock.UtcNow,
            cancellationToken);

        if (revokeResult.IsFailure)
            return Result<Unit>.Failure(revokeResult.Error);

        return Result<Unit>.Success(Unit.Value);
    }
}
