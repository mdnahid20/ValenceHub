using FluentValidation;
using ValenceHub.Domain.Common.ValueObjects;

namespace ValenceHub.Application.Features.Users.Commands.CreateUser;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    private const int MinPasswordLength = 8;

    public CreateUserCommandValidator()
    {
        RuleFor(x => x)
            .Must(HaveAtLeastOneContact)
            .WithMessage("Either email or phone number is required.");

        RuleFor(x => x.Email)
            .MaximumLength(320)
            .Must(BeValidEmail)
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Email must be a valid email address.");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(30)
            .Must(BeValidPhoneNumber)
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
            .WithMessage("Phone number must be valid.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(MinPasswordLength)
            .Matches("[A-Z]")
            .Matches("[a-z]")
            .Matches("[0-9]")
            .Matches("[!@#$%^&*()_+\\-=\\[\\]{};':\",./<>?]")
            .MaximumLength(255);
    }

    private static bool HaveAtLeastOneContact(CreateUserCommand command)
        => !string.IsNullOrWhiteSpace(command.Email) || !string.IsNullOrWhiteSpace(command.PhoneNumber);

    private static bool BeValidEmail(string? email)
        => string.IsNullOrWhiteSpace(email) || Email.Create(email).IsSuccess;

    private static bool BeValidPhoneNumber(string? phoneNumber)
        => string.IsNullOrWhiteSpace(phoneNumber) || PhoneNumber.Create(phoneNumber).IsSuccess;
}
