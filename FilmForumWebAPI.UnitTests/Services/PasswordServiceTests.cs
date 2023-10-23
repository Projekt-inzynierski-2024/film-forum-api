using FilmForumWebAPI.Services.Interfaces;
using FluentAssertions;

namespace FilmForumWebAPI.UnitTests.Services;

public class PasswordServiceTests
{
    [Fact]
    public void HashPassword_ForValidData_ReturnsHashedPassword()
    {
        //Arrange
        PasswordService passwordService = new();
        string password = "MyPassword123";
        //Act
        string hashedPassword = passwordService.HashPassword(password);

        //Assert
        hashedPassword.Should().NotBe(password);
    }

    [Fact]
    public void VerifyPassword_ForValidData_ReturnsTrue()
    {
        //Arrange
        PasswordService passwordService = new();
        string password = "MyPassword123";

        //Act
        string hashedPassword = passwordService.HashPassword(password);
        bool result = passwordService.VerifyPassword(password, hashedPassword);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HashPassword_ForNull_ThrowsArgumentNullException()
    {
        //Arrange
        PasswordService passwordService = new();
        string password = null!;

        //Act
        Action action = () => passwordService.HashPassword(password);

        //Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void VerifyPassword_ForNullPassword_ThrowsArgumentNullException()
    {
        //Arrange
        PasswordService passwordService = new();
        string password = null!;
        string hashedPassword = "NotEmptyHashedPassword";
        //Act
        Action action = () => passwordService.VerifyPassword(password, hashedPassword);

        //Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void VerifyPassword_ForNullHashedPassword_ThrowsArgumentNullException()
    {
        //Arrange
        PasswordService passwordService = new();
        string password = "NotEmptyPassword";
        string hashedPassword = null!;
        //Act
        Action action = () => passwordService.VerifyPassword(password, hashedPassword);

        //Assert
        action.Should().Throw<ArgumentNullException>();
    }
}