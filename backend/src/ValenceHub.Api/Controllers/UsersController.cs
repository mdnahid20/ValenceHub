using Microsoft.AspNetCore.Mvc;
using ValenceHub.Api.Contracts.Users;
using ValenceHub.Api.Extensions;
using ValenceHub.Api.Mappings;
using ValenceHub.Application.Messaging;
using ValenceHub.Persistence.Read.Queries.Users.GetUserById;

namespace ValenceHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    public UsersController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _commandDispatcher.Dispatch(request.ToCommand(), cancellationToken);
        return result.ToResponse(this);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUser(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _queryDispatcher.DispatchAsync(new GetUserByIdQuery { UserId = userId }, cancellationToken);
        return result.ToApiResponse(this);
    }
}