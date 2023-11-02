using PasswordManager.Interfaces;
using System.Security.Cryptography;
using FilmForumModels.Models.Password;
using FilmForumModels.Models.Exceptions;

namespace PasswordManager.Services;

public class PasswordResetTokenService : IPasswordResetTokenService
{
    private static string CreateToken() => Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

    private static DateTime CreateExpirationDate(int tokenLifetimeDays = 1)
        => tokenLifetimeDays >= 1 ? DateTime.Now.AddDays(tokenLifetimeDays) : throw new ResetPasswordTokenException();

    /// <summary>
    /// Creates token with expiration date based on <paramref name="tokenLifetimeDays"/> and <see cref="DateTime.Now"/>.
    /// </summary>
    /// <param name="tokenLifetimeDays">Quantity of days when token is valid and can be used</param>
    /// <returns>Token with expiration date</returns>
    /// <exception cref="ResetPasswordTokenException">If <paramref name="tokenLifetimeDays"/> is less than 1</exception>
    public PasswordResetTokenWithExpirationDate CreatePasswordResetTokenWithExpirationDate(int tokenLifetimeDays) => new()
    {
        Token = CreateToken(),
        ExpirationDate = CreateExpirationDate(tokenLifetimeDays)
    };
}