using Microsoft.AspNetCore.Mvc;
using ValenceHub.Application.Features.Users.Commands.CreateUser;
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Password = request.Password
        };

        var result = await _commandDispatcher.Dispatch(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error?.Message });
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
