namespace ValenceHub.Api.Contracts.Users;

public sealed record CreateUserRequest(
    string Email,
    string PhoneNumber,
    string Password);