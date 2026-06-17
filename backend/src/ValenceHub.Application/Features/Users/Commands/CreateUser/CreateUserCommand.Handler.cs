using Ardalis.GuardClauses;
using ValenceHub.Application.Abstractions.Commands;
using ValenceHub.Application.Abstractions.Repositories;
using ValenceHub.Application.Abstractions.Results;
using ValenceHub.Application.Abstractions.Services;
using ValenceHub.Application.Features.Auth.Abstractions;
using ValenceHub.Domain.Common.ValueObjects;
using ValenceHub.Domain.Otps;
using ValenceHub.Domain.Users;
using ValenceHub.Domain.Otps.Enums;
using ValenceHub.Domain.Common.Enums;
using ValenceHub.Application.Abstractions.Transactions;

namespace ValenceHub.Application.Features.Users.Commands.CreateUser;

public sealed class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IOtpService _otpService;
    private readonly IDateTimeOffsetProvider _clock;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IOtpService otpService,
        IDateTimeOffsetProvider clock,
        IUnitOfWork unitOfWork)
    {
        _userRepository = Guard.Against.Null(userRepository);
        _passwordHasher = Guard.Against.Null(passwordHasher);
        _otpService = Guard.Against.Null(otpService);
        _clock = Guard.Against.Null(clock);
        _unitOfWork = Guard.Against.Null(unitOfWork);
    }

    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken ct)
    {
        Email? email = null;
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var emailResult = Email.Create(request.Email);
            if (emailResult.IsFailure)
                return emailResult.Error.ToResult<Guid>();

            email = emailResult.Value;
        }

        PhoneNumber? phone = null;
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            var phoneResult = PhoneNumber.Create(request.PhoneNumber);
            if (phoneResult.IsFailure)
                return phoneResult.Error.ToResult<Guid>();

            phone = phoneResult.Value;
        }
        //TODO : Currently We only work with Email OTP System.
        if (email is null)
        {
            return Result<Guid>.Failure(
                Error.NotSupported(
                    "Auth.Register.EmailRequired",
                    "Email registration is currently required while phone OTP support is paused."));
        }

        var password = Password.Create(request.Password);
        if (password.IsFailure)
            return password.Error.ToResult<Guid>();

        if (email is not null && await _userRepository.ExistsByEmailAsync(email, ct))
            return Result<Guid>.Failure(Error.Validation("Email.Exists", "Email already exists"));

        if (phone is not null && await _userRepository.ExistsByPhoneNumberAsync(phone, ct))
            return Result<Guid>.Failure(Error.Validation("Phone.Exists", "Phone already exists"));

        var user = User.Create(
            email,
            phone,
            _passwordHasher.HashPassword(password.Value.Value),
            UserId.New(),
            _clock.UtcNow);

        if (user.IsFailure)
            return user.Error.ToResult<Guid>();

        await _userRepository.AddAsync(user.Value, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var sendOtpResult = await _otpService.IssueAsync(
            user.Value.Id.Value,
            CommunicationChannel.Email,
            email!.Value,
            OtpPurpose.Register,
            ct);

        if (sendOtpResult.IsFailure)
            return Result<Guid>.Failure(sendOtpResult.Error);

        return Result<Guid>.Success(user.Value.Id.Value);
    }
}

