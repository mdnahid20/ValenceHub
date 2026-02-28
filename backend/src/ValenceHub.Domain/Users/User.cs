using ValenceHub.Domain.Abstractions;
using ValenceHub.Domain.Common.Results;
using ValenceHub.Domain.Common.ValueObjects;
using ValenceHub.Domain.Interfaces;

namespace ValenceHub.Domain.Users;

public sealed class User : AggregateRoot<UserId>, IAuditable, ISoftDelete
{
    private const int MaxPasswordHashLength = 255;
    private static class UserErrors
    {
        public static readonly Error MissingContact =
            Error.Validation(
                "User.Contact.Required",
                "A user must have either an email or a phone number.");

        public static readonly Error PasswordRequired =
            Error.Validation("User.PasswordHash.Required", "Password hash is required.");

        public static Error PasswordTooLong(int maxLength) =>
            Error.Validation(
                "User.PasswordHash.TooLong",
                $"Password hash must be at most {maxLength} characters.");
    }

    private User(
        UserId id,
        Email? email,
        PhoneNumber? phoneNumber,
        string passwordHash,
        DateTime createdAt,
        DateTime? updatedAt,
        bool isDeleted)
        : base(id)
    {
        Email = email;
        PhoneNumber = phoneNumber;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        IsDeleted = isDeleted;
    }

    public Email? Email { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public string PasswordHash { get; private set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    public static Result<User> Create(string? email, string? phoneNumber, string passwordHash)
    {
        Email? emailVo = null;
        if (!string.IsNullOrWhiteSpace(email))
        {
            var emailResult = Email.Create(email);
            if (emailResult.IsFailure)
                return Result<User>.Failure(emailResult.Error);

            emailVo = emailResult.Value;
        }

        PhoneNumber? phoneVo = null;
        if (!string.IsNullOrWhiteSpace(phoneNumber))
        {
            var phoneResult = PhoneNumber.Create(phoneNumber);
            if (phoneResult.IsFailure)
                return Result<User>.Failure(phoneResult.Error);

            phoneVo = phoneResult.Value;
        }

        var contactResult = EnsureContactProvided(emailVo, phoneVo);
        if (contactResult.IsFailure)
            return Result<User>.Failure(contactResult.Error);

        var passwordResult = ValidatePasswordHash(passwordHash);
        if (passwordResult.IsFailure)
            return Result<User>.Failure(passwordResult.Error);

        var now = DateTime.UtcNow;

        return Result<User>.Success(new User(
            id: UserId.New(),
            email: emailVo,
            phoneNumber: phoneVo,
            passwordHash: passwordResult.Value,
            createdAt: now,
            updatedAt: now,
            isDeleted: false));
    }

    public Result UpdateContact(string? email, string? phoneNumber)
    {
        Email? emailVo = null;
        if (!string.IsNullOrWhiteSpace(email))
        {
            var emailResult = Email.Create(email);
            if (emailResult.IsFailure)
                return Result.Failure(emailResult.Error);

            emailVo = emailResult.Value;
        }

        PhoneNumber? phoneVo = null;
        if (!string.IsNullOrWhiteSpace(phoneNumber))
        {
            var phoneResult = PhoneNumber.Create(phoneNumber);
            if (phoneResult.IsFailure)
                return Result.Failure(phoneResult.Error);

            phoneVo = phoneResult.Value;
        }

        var contactResult = EnsureContactProvided(emailVo, phoneVo);
        if (contactResult.IsFailure)
            return Result.Failure(contactResult.Error);

        Email = emailVo;
        PhoneNumber = phoneVo;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public Result UpdatePasswordHash(string passwordHash)
    {
        var passwordResult = ValidatePasswordHash(passwordHash);
        if (passwordResult.IsFailure)
            return Result.Failure(passwordResult.Error);

        PasswordHash = passwordResult.Value;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    private static Result EnsureContactProvided(Email? email, PhoneNumber? phoneNumber)
    {
        if (email is null && phoneNumber is null)
            return Result.Failure(UserErrors.MissingContact);

        return Result.Success();
    }

    private static Result<string> ValidatePasswordHash(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<string>.Failure(UserErrors.PasswordRequired);

        if (value.Length > MaxPasswordHashLength)
            return Result<string>.Failure(UserErrors.PasswordTooLong(MaxPasswordHashLength));

        return Result<string>.Success(value);
    }
}
