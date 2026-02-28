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

        public static readonly Error CannotModifyDeletedUser =
            Error.Validation(
                "User.Deleted.ModificationNotAllowed",
                "Cannot modify a deleted user.");
    }

    private User(
        UserId id,
        Email? email,
        PhoneNumber? phoneNumber,
        string passwordHash,
        UserId? createdBy,   
        DateTime createdAt,
        bool isDeleted)
        : base(id)
    {
        Email = email;
        PhoneNumber = phoneNumber;
        PasswordHash = passwordHash;

        CreatedBy = createdBy;  
        CreatedAt = createdAt;
        
        IsDeleted = isDeleted;
    }

    public Email? Email { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public string PasswordHash { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public UserId? CreatedBy { get; private set; }
    public UserId? UpdatedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public UserId? DeletedBy { get; private set; }

    public static Result<User> Create(string? email, string? phoneNumber, string passwordHash,UserId createdBy, DateTime now)
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

        return Result<User>.Success(new User(
            id: UserId.New(),
            email: emailVo,
            phoneNumber: phoneVo,
            passwordHash: passwordResult.Value,
            createdBy: createdBy,
            createdAt: now,
            isDeleted: false));
    }

    public Result UpdateContact(string? email, string? phoneNumber,UserId updateBy, DateTime now)
    {
        if (IsDeleted)
            return Result.Failure(UserErrors.CannotModifyDeletedUser);

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
        UpdatedAt = now;
        UpdatedBy = updateBy;

        return Result.Success();
    }

    public Result UpdatePasswordHash(string passwordHash, UserId updateBy, DateTime now)
    {
        if (IsDeleted)
            return Result.Failure(UserErrors.CannotModifyDeletedUser);

        var passwordResult = ValidatePasswordHash(passwordHash);
        if (passwordResult.IsFailure)
            return Result.Failure(passwordResult.Error);

        PasswordHash = passwordResult.Value;
        UpdatedAt = now;
        UpdatedBy = updateBy;   

        return Result.Success();
    }
    public Result Delete(UserId deletedBy, DateTime now)
    {
        if (IsDeleted)
            return Result.Success();

        IsDeleted = true;
        DeletedAt = now;
        DeletedBy = deletedBy;

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
