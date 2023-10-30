namespace PasswordManager.Interfaces;

public interface IPasswordService
{
    bool VerifyPassword(string password, string hashedPassword);

    string HashPassword(string password);
}