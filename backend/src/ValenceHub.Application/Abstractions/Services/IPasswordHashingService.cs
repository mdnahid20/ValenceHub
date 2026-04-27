namespace ValenceHub.Application.Abstractions.Services;

public interface IPasswordHashingService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string storedHash);
}
