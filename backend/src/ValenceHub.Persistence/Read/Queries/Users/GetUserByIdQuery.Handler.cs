using Ardalis.GuardClauses;
using ValenceHub.Application.Abstractions.Queries;
using ValenceHub.Application.Abstractions.Results;
using ValenceHub.Persistence.Read.Repositories.Users;

namespace ValenceHub.Persistence.Read.Queries.Users.GetUserById;

public sealed class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, GetUserByIdResponse>
{
    private readonly IUserReadRepository _userReadRepository;

    public GetUserByIdQueryHandler(IUserReadRepository userReadRepository)
    {
        _userReadRepository = Guard.Against.Null(userReadRepository);
    }

    public async Task<Result<GetUserByIdResponse>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await HandleAsync(request, cancellationToken);
    }

    private async Task<Result<GetUserByIdResponse>> HandleAsync(
        GetUserByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var user = await _userReadRepository.GetByIdAsync(query.UserId, cancellationToken);

        if (user is null)
        {
            return Result<GetUserByIdResponse>.Failure(
                Error.NotFound("User.NotFound", $"User with ID {query.UserId} not found"));
        }

        var response = new GetUserByIdResponse(
            Id: user.Id,
            Email: user.Email ?? string.Empty,
            PhoneNumber: user.PhoneNumber ?? string.Empty);

        return Result<GetUserByIdResponse>.Success(response);
    }
}


