using FilmForumModels.Models.Exceptions;
using FilmForumModels.Models.Password;
using FluentAssertions;
using PasswordManager.Interfaces;
using PasswordManager.Services;

namespace PasswordManager.UnitTests.Services;

public class PasswordResetTokenServiceTests
{
    private readonly IPasswordResetTokenService _passwordResetTokenService = new PasswordResetTokenService();

    [Fact]
    public void CreatePasswordResetTokenWithExpirationDate_ForDefaultTokenLifetime_ShouldReturnWithExpirationDate()
    {
        // Act
        PasswordResetTokenWithExpirationDate token = _passwordResetTokenService.CreatePasswordResetTokenWithExpirationDate();

        // Assert
        token.Token.Should().NotBeNullOrEmpty();
        token.ExpirationDate.Should().BeAfter(DateTime.Now);
    }

    [Fact]
    public void CreatePasswordResetTokenWithExpirationDate_ForGivenTokenLifetime_ShouldReturnWithExpirationDate()
    {
        // Act
        PasswordResetTokenWithExpirationDate token = _passwordResetTokenService.CreatePasswordResetTokenWithExpirationDate(2);

        // Assert
        token.Token.Should().NotBeNullOrEmpty();
        token.ExpirationDate.Should().BeAfter(DateTime.Now);
    }

    [Fact]
    public void CreatePasswordResetTokenWithExpirationDate_ForInvalidTokenLifetime_ShouldThrowResetPasswordTokenException()
    {
        // Act
        Action action = () => _passwordResetTokenService.CreatePasswordResetTokenWithExpirationDate(0);

        // Act
        action.Should().Throw<ResetPasswordTokenException>();
    }
}