using AuthenticationManager.Services;
using FilmForumModels.Dtos.UserDtos;
using FilmForumModels.Entities;
using FilmForumModels.Models.Settings;
using FilmForumWebAPI.Database;
using FilmForumWebAPI.IntegrationTests.HelpersForTests;
using FilmForumWebAPI.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using PasswordManager.Services;

namespace FilmForumWebAPI.IntegrationTests.Services;

public class UserServiceTests
{
    [Theory]
    [InlineData(1, true)]
    [InlineData(999999, false)]
    public async Task UserWithIdExistsAsync_ForGivenId_ReturnsTrueIfUserExistsOtherwiseFalse(int userId, bool expected)
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTesting();
        UserService userService = new(usersDatabaseContext, new PasswordService(), new JwtService(), Options.Create(new JwtDetails()), new RoleService(usersDatabaseContext));
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "dasd321D!@#@!#a", Email = "test@test.com", Username = "name" });
        await usersDatabaseContext.SaveChangesAsync();

        //Act
        bool userExists = await userService.UserWithIdExistsAsync(userId);

        //Assert
        userExists.Should().Be(expected);
    }

    [Theory]
    [InlineData("name", true)]
    [InlineData("user who do not exists", false)]
    public async Task UserWithUsernameExistsAsync_ForGivenUsername_ReturnsTrueIfUserExistsOtherwiseFalse(string username, bool expected)
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTesting();
        UserService userService = new(usersDatabaseContext, new PasswordService(), new JwtService(), Options.Create(new JwtDetails()), new RoleService(usersDatabaseContext));
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "dasd321D!@#@!#a", Email = "test@test.com", Username = "name" });
        await usersDatabaseContext.SaveChangesAsync();

        //Act
        bool userExists = await userService.UserWithUsernameExistsAsync(username);

        //Assert
        userExists.Should().Be(expected);
    }

    [Theory]
    [InlineData("test@test.com", true)]
    [InlineData("userwhodonotexists@email.com", false)]
    public async Task UserWithEmailExistsAsync_ForGivenEmail_ReturnsTrueIfUserExistsOtherwiseFalse(string email, bool expected)
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTesting();
        UserService userService = new(usersDatabaseContext, new PasswordService(), new JwtService(), Options.Create(new JwtDetails()), new RoleService(usersDatabaseContext));
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "dasd321D!@#@!#a", Email = "test@test.com", Username = "name" });
        await usersDatabaseContext.SaveChangesAsync();

        //Act
        bool userExists = await userService.UserWithEmailExistsAsync(email);

        //Assert
        userExists.Should().Be(expected);
    }

    [Fact]
    public async Task GetAllAsync_ForExistingUsers_ReturnsAllUsers()
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTesting();
        UserService userService = new(usersDatabaseContext, new PasswordService(), new JwtService(), Options.Create(new JwtDetails()), new RoleService(usersDatabaseContext));
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "pdsade23#!@!DA2", Email = "ememail@da.pl", Username = "nickanem1232" });
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "dadada2#!@#@DAas", Email = "e2m@email.pl", Username = "name" });
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "da@#!@#a", Email = "em1@emailtest.pl", Username = "name12" });
        await usersDatabaseContext.SaveChangesAsync();

        //Act
        List<GetUserDto> users = await userService.GetAllAsync();

        //Assert
        users.Count.Should().Be(3);
    }

    [Fact]
    public async Task GetAsync_ForExistingUser_ReturnsUser()
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTesting();
        UserService userService = new(usersDatabaseContext, new PasswordService(), new JwtService(), Options.Create(new JwtDetails()), new RoleService(usersDatabaseContext));
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "pdsade23#!@!DA2", Email = "ememail@da.pl", Username = "nickanem1232" });
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "dadada2#!@#@DAas", Email = "e2m@email.pl", Username = "name" });
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "da@#!@#a", Email = "em1@emailtest.pl", Username = "name12" });
        await usersDatabaseContext.SaveChangesAsync();

        //Act
        GetUserDto? user = await userService.GetAsync(1);

        //Assert
        user.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAsync_ForNonExistingUser_ReturnsNull()
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTesting();
        UserService userService = new(usersDatabaseContext, new PasswordService(), new JwtService(), Options.Create(new JwtDetails()), new RoleService(usersDatabaseContext));
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "pdsade23#!@!DA2", Email = "ememail@da.pl", Username = "nickanem1232" });
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "dadada2#!@#@DAas", Email = "e2m@email.pl", Username = "name" });
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "da@#!@#a", Email = "em1@emailtest.pl", Username = "name12" });
        await usersDatabaseContext.SaveChangesAsync();

        //Act
        GetUserDto? user = await userService.GetAsync(999999);

        //Assert
        user.Should().BeNull();
    }

    [Theory]
    [InlineData(1, "NewPassword123!", "NewPassword123!", 1)]
    [InlineData(999999, "NewPassword123!", "NewPassword123!", 0)]
    public async Task ChangePasswordAsync_ForGivenData_ChangesPasswordIfUserExists(int userId, string password, string confirmPassword, int expected)
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTesting();
        UserService userService = new(usersDatabaseContext, new PasswordService(), new JwtService(), Options.Create(new JwtDetails()), new RoleService(usersDatabaseContext));
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "da@#!@#a", Email = "email@emailtest.pl", Username = "name12" });
        await usersDatabaseContext.SaveChangesAsync();

        //Act
        int result = await userService.ChangePasswordAsync(userId, new(password, confirmPassword));

        //Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("emailtest@emailtest.pl", "password!KJ", "password!KJ", "token123", 1)]
    [InlineData("invalid@emailtest.pl", "password!KJ", "password!KJ", "token123", 0)]
    public async Task ResetPasswordAsync_ForGivenData_ResetsPasswordIfUserExists(string email, string password, string confirmPassword, string resetPasswordToken, int expected)
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTesting();
        UserService userService = new(usersDatabaseContext, new PasswordService(), new JwtService(), Options.Create(new JwtDetails()), new RoleService(usersDatabaseContext));
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "da@#!@#a", Email = "emailtest@emailtest.pl", Username = "name12" });
        await usersDatabaseContext.SaveChangesAsync();

        //Act
        int result = await userService.ResetPasswordAsync(new(email, password, confirmPassword, resetPasswordToken));

        //Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(1, "newemail@newemail.com", 1)]
    [InlineData(99999, "newemail@newemail.com", 0)]
    public async Task ChangeEmailAsync_ForGivenData_ChangesEmailIfUserExists(int userId, string email, int expected)
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTesting();
        UserService userService = new(usersDatabaseContext, new PasswordService(), new JwtService(), Options.Create(new JwtDetails()), new RoleService(usersDatabaseContext));
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "da@#!@#a", Email = "emailtest@emailtest.pl", Username = "name12" });
        await usersDatabaseContext.SaveChangesAsync();

        //Act
        int result = await userService.ChangeEmailAsync(userId, email);

        //Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(1, "NewUsername", 1)]
    [InlineData(99999, "NewUsername", 0)]
    public async Task ChangeUsernameAsync_ForGivenData_ChangesUsernameIfUserExists(int userId, string username, int expected)
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTesting();
        UserService userService = new(usersDatabaseContext, new PasswordService(), new JwtService(), Options.Create(new JwtDetails()), new RoleService(usersDatabaseContext));
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "da@#!@#a", Email = "emailtest@emailtest.pl", Username = "name12" });
        await usersDatabaseContext.SaveChangesAsync();

        //Act
        int result = await userService.ChangeUsernameAsync(userId, username);

        //Assert
        result.Should().Be(expected);
    }
}