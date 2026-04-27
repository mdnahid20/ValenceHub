using FluentValidation;
using ValenceHub.Application.Abstractions.Repositories;

namespace ValenceHub.Application.Features.Users.Commands.CreateUser;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    private const int MaxNameLength = 100;
    private const int MinPasswordLength = 8;

    public CreateUserCommandValidator(IUserRepository userRepository)
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(MaxNameLength)
            .WithMessage($"First name cannot exceed {MaxNameLength} characters")
            .Matches(@"^[a-zA-Z\s\-']+$")
            .WithMessage("First name contains invalid characters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(MaxNameLength)
            .WithMessage($"Last name cannot exceed {MaxNameLength} characters")
            .Matches(@"^[a-zA-Z\s\-']+$")
            .WithMessage("Last name contains invalid characters");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(MinPasswordLength)
            .WithMessage($"Password must be at least {MinPasswordLength} characters")
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]")
            .WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]")
            .WithMessage("Password must contain at least one digit")
            .Matches("[!@#$%^&*()_+\\-=\\[\\]{};':\",./<>?]")
            .WithMessage("Password must contain at least one special character")
            .MaximumLength(255)
            .WithMessage("Password cannot exceed 255 characters");

        RuleFor(x => x)
            .Must(HaveEmailOrPhone)
            .WithMessage("Either email address or phone number is required")
            .WithName("Contact Information");

        RuleFor(x => x.Email)
            .MustAsync(async (email, ct) =>
                string.IsNullOrWhiteSpace(email) || !await userRepository.ExistsByEmailAsync(email, ct))
            .WithMessage("Email already exists")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.PhoneNumber)
            .MustAsync(async (phone, ct) =>
                string.IsNullOrWhiteSpace(phone) || !await userRepository.ExistsByPhoneAsync(phone, ct))
            .WithMessage("Phone number already exists")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
    }

    private static bool HaveEmailOrPhone(CreateUserCommand command)
    {
        var hasEmail = !string.IsNullOrWhiteSpace(command.Email);
        var hasPhone = !string.IsNullOrWhiteSpace(command.PhoneNumber);
        return hasEmail || hasPhone;
    }
}

