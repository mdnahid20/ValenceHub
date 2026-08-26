namespace ValenceHub.Persistence.Read.Queries.Users.GetUserById;

public sealed record GetUserByIdResponse(
    Guid Id,
    string Email,
    string PhoneNumber);
