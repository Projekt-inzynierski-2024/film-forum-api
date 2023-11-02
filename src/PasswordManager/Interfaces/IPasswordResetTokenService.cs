using FilmForumModels.Models.Password;

namespace PasswordManager.Interfaces;

public interface IPasswordResetTokenService
{
    PasswordResetTokenWithExpirationDate CreatePasswordResetTokenWithExpirationDate(int tokenLifetimeDays = 1);
}