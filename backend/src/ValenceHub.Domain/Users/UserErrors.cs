using ValenceHub.Domain.Common.Results;

namespace ValenceHub.Domain.Users;

public static class UserErrors
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

    public static readonly Error EmailAlreadyExists =
        Error.Validation("User.EmailExists", "Email already exists");

    public static readonly Error PhoneAlreadyExists =
        Error.Validation("User.PhoneExists", "Phone number already exists");

    public static Error DuplicateEmail(string email) =>
        EmailAlreadyExists;

    public static Error DuplicatePhone(string phoneNumber) =>
        PhoneAlreadyExists;
}
