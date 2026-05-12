using Ardalis.GuardClauses;
using ValenceHub.Application.Abstractions.Commands;
using ValenceHub.Application.Abstractions.Repositories;
using ValenceHub.Application.Abstractions.Results;
using ValenceHub.Application.Abstractions.Services;
using ValenceHub.Application.Abstractions.Transactions;
using ValenceHub.Application.Features.Auth.Abstractions;
using ValenceHub.Domain.Common.ValueObjects;
using ValenceHub.Domain.Users;

namespace ValenceHub.Application.Features.Users.Commands.CreateUser;

public sealed class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDateTimeOffsetProvider _clock;
    private readonly IUnitOfWork _unitOfWork;
    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IDateTimeOffsetProvider clock,
        IUnitOfWork unitOfWork)
    {
        _userRepository = Guard.Against.Null(userRepository);
        _passwordHasher = Guard.Against.Null(passwordHasher);
        _clock = Guard.Against.Null(clock);
        _unitOfWork = Guard.Against.Null(unitOfWork);
    }

    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken ct)
    {
        var email = Email.Create(request.Email);
        if (email.IsFailure)
            return email.Error.ToResult<Guid>();

        var phone = PhoneNumber.Create(request.PhoneNumber);
        if (phone.IsFailure)
            return phone.Error.ToResult<Guid>();

        var password = Password.Create(request.Password);
        if (password.IsFailure)
            return password.Error.ToResult<Guid>();

        if (await _userRepository.ExistsByEmailAsync(email.Value, ct))
            return Result<Guid>.Failure(Error.Validation("Email.Exists", "Email already exists"));

        if (await _userRepository.ExistsByPhoneNumberAsync(phone.Value, ct))
            return Result<Guid>.Failure(Error.Validation("Phone.Exists", "Phone already exists"));

        var user = User.Create(
            email.Value,
            phone.Value,
            _passwordHasher.HashPassword(password.Value.Value),
            UserId.New(),
            _clock.UtcNow);

        if (user.IsFailure)
            return user.Error.ToResult<Guid>();

        await _userRepository.AddAsync(user.Value, ct);
        await _unitOfWork.SaveChangesAsync(ct); 

        return Result<Guid>.Success(user.Value.Id.Value);
    }
}

