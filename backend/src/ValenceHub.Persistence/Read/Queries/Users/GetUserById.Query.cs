using ValenceHub.Application.Abstractions.Queries;

namespace ValenceHub.Persistence.Read.Queries.Users.GetUserById;

public sealed record GetUserByIdQuery : IQuery<GetUserByIdResponse>
{
    public required Guid UserId { get; init; }
}

public sealed record GetUserByIdResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber);
