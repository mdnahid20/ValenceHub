using Microsoft.AspNetCore.Identity.Data;
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
        return result.ToJsonResponse();
        }

        return Ok(new { userId = result.Value });
    }

    [HttpGet("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUser(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery { UserId = userId };

        var result = await _queryDispatcher.DispatchAsync(query, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(new { error = result.Error?.Message });
        }

        return Ok(result);
    }
}

public sealed record CreateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Password);
