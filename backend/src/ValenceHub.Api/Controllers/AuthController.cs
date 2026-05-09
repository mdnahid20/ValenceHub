using Microsoft.AspNetCore.Mvc;
using ValenceHub.Api.Contracts.Auth;
using ValenceHub.Api.Extensions;
using ValenceHub.Api.Mappings;
using ValenceHub.Application.Messaging;

namespace ValenceHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;

    public AuthController(ICommandDispatcher commandDispatcher)
    {
        _commandDispatcher = commandDispatcher;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand();

        var result = await _commandDispatcher.Dispatch(
            command,
            cancellationToken);

        return result.ToApiResponse(this);
    }
}