using ValenceHub.Domain.Abstractions;
using ValenceHub.Domain.Common.Results;
using ValenceHub.Domain.Common.ValueObjects;
using ValenceHub.Domain.Interfaces;
using ValenceHub.Domain.Users.Events;

namespace ValenceHub.Domain.Users;

public sealed class User : AggregateRoot<UserId,Guid>, IAuditable, ISoftDelete
{
    private const int MaxPasswordHashLength = 255;

    private User(
        UserId id,
        Email? email,
        PhoneNumber? phoneNumber,
        string passwordHash,
        UserId? createdBy,   
        DateTimeOffset createdAt,
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

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }
    public UserId? CreatedBy { get; private set; }
    public UserId? UpdatedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public UserId? DeletedBy { get; private set; }

    public static Error? Create(
        Email? email,
        PhoneNumber? phoneNumber,
        string passwordHash,
        UserId createdBy,
        DateTimeOffset now,
        out User? user)
    {
        user = null;

        var contactError = EnsureContactProvided(email, phoneNumber);
        if (contactError is not null)
            return contactError;

        var passwordError = ValidatePasswordHash(passwordHash);
        if (passwordError is not null)
            return passwordError;

        user = new User(
            id: UserId.New(),
            email: email,
            phoneNumber: phoneNumber,
            passwordHash: passwordHash,
            createdBy: createdBy,
            createdAt: now,
            isDeleted: false);

        user.RaiseDomainEvent(
        new UserCreatedDomainEvent(
            user.Id,
            createdBy,
            user.Email?.Value,
            user.PhoneNumber?.Value,
            now));

        return null;
    }

    public static Result<User> Create(
        Email? email,
        PhoneNumber? phoneNumber,
        string passwordHash,
        UserId createdBy,
        DateTimeOffset now)
    {
        var error = Create(email, phoneNumber, passwordHash, createdBy, now, out var user);
        return error is null
            ? Result<User>.Success(user!)
            : Result<User>.Failure(error);
    }

    public Error? UpdateContact(Email? email, PhoneNumber? phoneNumber, UserId updateBy, DateTimeOffset now)
    {
        if (IsDeleted)
            return UserErrors.CannotModifyDeletedUser;

        var contactError = EnsureContactProvided(email, phoneNumber);
        if (contactError is not null)
            return contactError;

        Email = email;
        PhoneNumber = phoneNumber;
        UpdatedAt = now;
        UpdatedBy = updateBy;

        RaiseDomainEvent(new UserContactUpdatedDomainEvent( Id,updateBy,Email?.Value, PhoneNumber?.Value, now));

        return null;
    }

    public Error? UpdatePasswordHash(string passwordHash, UserId updateBy, DateTimeOffset now)
    {
        if (IsDeleted)
            return UserErrors.CannotModifyDeletedUser;

        var passwordError = ValidatePasswordHash(passwordHash);
        if (passwordError is not null)
            return passwordError;

        PasswordHash = passwordHash;
        UpdatedAt = now;
        UpdatedBy = updateBy;

        RaiseDomainEvent(new UserPasswordChangedDomainEvent(Id, now));
        return null;
    }
    public Error? Delete(UserId deletedBy, DateTimeOffset now)
    {
        if (IsDeleted)
            return null;

        IsDeleted = true;
        DeletedAt = now;
        DeletedBy = deletedBy;

        RaiseDomainEvent(new UserDeletedDomainEvent(Id, deletedBy, now));

        return null;
    }
    private static Error? EnsureContactProvided(Email? email, PhoneNumber? phoneNumber)
    {
        if (email is null && phoneNumber is null)
            return UserErrors.MissingContact;

        return null;
    }

    private static Error? ValidatePasswordHash(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return UserErrors.PasswordRequired;

        if (value.Length > MaxPasswordHashLength)
            return UserErrors.PasswordTooLong(MaxPasswordHashLength);

        return null;
    }
}
