using AuthenticationManager.Services;
using FilmForumModels.Dtos.UserDtos;
using FilmForumModels.Entities;
using FilmForumModels.Models.Enums;
using FilmForumModels.Models.Exceptions;
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
    [InlineData(1, true, 1)]
    [InlineData(1, false, 1)]
    [InlineData(999999, false, 0)]
    [InlineData(999999, true, 0)]
    public async Task ChangeMultifactorAuthAsync_ForGivenId_ChangesMultifactorAuthIfUserExists(int userId, bool multifactorAuth, int expected)
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTestingAsync();
        UserService userService = new(usersDatabaseContext, new PasswordService(), new JwtService(), Options.Create(new JwtDetails()), new RoleService(usersDatabaseContext), new MultifactorAuthenticationService());
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "dasd321D!@#@!#a", Email = "test@test.com", Username = "name" });
        await usersDatabaseContext.SaveChangesAsync();

        //Act
        int result = await userService.ChangeMultifactorAuthAsync(userId, multifactorAuth);

        //Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(999999, false)]
    public async Task UserWithIdExistsAsync_ForGivenId_ReturnsTrueIfUserExistsOtherwiseFalse(int userId, bool expected)
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTestingAsync();
        UserService userService = new(usersDatabaseContext, new PasswordService(), new JwtService(), Options.Create(new JwtDetails()), new RoleService(usersDatabaseContext), new MultifactorAuthenticationService());
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
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTestingAsync();
        UserService userService = new(usersDatabaseContext, new PasswordService(), new JwtService(), Options.Create(new JwtDetails()), new RoleService(usersDatabaseContext), new MultifactorAuthenticationService());
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
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTestingAsync();
        UserService userService = new(usersDatabaseContext, new PasswordService(), new JwtService(), Options.Create(new JwtDetails()), new RoleService(usersDatabaseContext), new MultifactorAuthenticationService());
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "dasd321D!@#@!#a", Email = "test@test.com", Username = "name" });
        await usersDatabaseContext.SaveChangesAsync();

        //Act
        bool userExists = await userService.UserWithEmailExistsAsync(email);

        //Assert
        userExists.Should().Be(expected);
    }

    [Theory]
    [InlineData("email@email.com", true, true)]
    [InlineData("email2@email.com", false, false)]
    [InlineData("emailwhodonotexists@email.com", true, false)]
    [InlineData("emailwhodonotexists@email.com", false, false)]
    public async Task UserWithEmailAndMultifactorAuthOnExistsAsync_ForGivenEmailAndAuth_ReturnsTrueIfUserExistsOtherwiseFalse(string email, bool auth, bool expected)
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTestingAsync();
        UserService userService = new(usersDatabaseContext, new PasswordService(), new JwtService(), Options.Create(new JwtDetails()), new RoleService(usersDatabaseContext), new MultifactorAuthenticationService());
        await usersDatabaseContext.Users.AddRangeAsync(new List<User>()
        {
            new() { Password = "dasd321D!@#@!#a", Email = "email@email.com", Username = "name", MultifactorAuth = auth },
            new() { Password = "dasd321D!@#@!#a2", Email = "email2@email.com", Username = "name2", MultifactorAuth = auth }
        });

        await usersDatabaseContext.SaveChangesAsync();

        //Act
        bool userExists = await userService.UserWithEmailAndMultifactorAuthOnExistsAsync(email);

        //Assert
        userExists.Should().Be(expected);
    }

    [Fact]
    public async Task GetAllAsync_ForExistingUsers_ReturnsAllUsers()
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTestingAsync();
        UserService userService = new(usersDatabaseContext, new PasswordService(), new JwtService(), Options.Create(new JwtDetails()), new RoleService(usersDatabaseContext), new MultifactorAuthenticationService());
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
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTestingAsync();
        UserService userService = new(usersDatabaseContext, new PasswordService(), new JwtService(), Options.Create(new JwtDetails()), new RoleService(usersDatabaseContext), new MultifactorAuthenticationService());
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
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTestingAsync();
        UserService userService = new(usersDatabaseContext, new PasswordService(), new JwtService(), Options.Create(new JwtDetails()), new RoleService(usersDatabaseContext), new MultifactorAuthenticationService());
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
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTestingAsync();
        UserService userService = new(usersDatabaseContext, new PasswordService(), new JwtService(), Options.Create(new JwtDetails()), new RoleService(usersDatabaseContext), new MultifactorAuthenticationService());
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
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTestingAsync();
        UserService userService = new(usersDatabaseContext, new PasswordService(), new JwtService(), Options.Create(new JwtDetails()), new RoleService(usersDatabaseContext), new MultifactorAuthenticationService());
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
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTestingAsync();
        UserService userService = new(usersDatabaseContext, new PasswordService(), new JwtService(), Options.Create(new JwtDetails()), new RoleService(usersDatabaseContext), new MultifactorAuthenticationService());
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
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTestingAsync();
        UserService userService = new(usersDatabaseContext, new PasswordService(), new JwtService(), Options.Create(new JwtDetails()), new RoleService(usersDatabaseContext), new MultifactorAuthenticationService());
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "da@#!@#a", Email = "emailtest@emailtest.pl", Username = "name12" });
        await usersDatabaseContext.SaveChangesAsync();

        //Act
        int result = await userService.ChangeUsernameAsync(userId, username);

        //Assert
        result.Should().Be(expected);
    }

    [Fact]
    public async Task CreateAsync_ForValidData_CreatesUser()
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTestingAsync();
        JwtDetails jwtDetails = new()
        {
            SecretKey = "SuperSecretKeyForJwtToken123123123456",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            LifetimeInMinutes = 60
        };
        UserService userService = new(usersDatabaseContext, new PasswordService(), new JwtService(), Options.Create(jwtDetails), new RoleService(usersDatabaseContext), new MultifactorAuthenticationService());
        CreateUserDto createUserDto = new("username", "email@email.com", "Password123!", "Password123!");

        //Act
        UserCreatedDto result = await userService.CreateAsync(createUserDto, UserRole.User);

        //Assert
        result.Id.Should().Be(1);
        result.Email.Should().Be(createUserDto.Email);
        result.Username.Should().Be(createUserDto.Username);
        result.Jwt.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CreateAsync_ForInvalidUserMainRole_ThrowsInvalidRoleNameException()
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTestingAsync();
        UserService userService = new(usersDatabaseContext, new PasswordService(), new JwtService(), Options.Create(new JwtDetails()), new RoleService(usersDatabaseContext), new MultifactorAuthenticationService());
        CreateUserDto createUserDto = new("username", "email@email.com", "Password123!", "Password123!");

        //Act
        Func<Task<UserCreatedDto>> action = async () => await userService.CreateAsync(createUserDto, (UserRole)999999);

        //Assert
        await action.Should().ThrowAsync<InvalidRoleNameException>();
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(99999, 0)]
    public async Task RemoveAsync_ForGivenId_DeletesUserIfExistsInDatabase(int userId, int expected)
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTestingAsync();
        UserService userService = new(usersDatabaseContext, new PasswordService(), new JwtService(), Options.Create(new JwtDetails()), new RoleService(usersDatabaseContext), new MultifactorAuthenticationService());
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "da@#!@#a", Email = "emailtest@emailtest.pl", Username = "name12" });
        await usersDatabaseContext.SaveChangesAsync();

        //Act
        int result = await userService.RemoveAsync(userId);

        //Assert
        result.Should().Be(expected);
    }

    [Fact]
    public async Task LogInAsync_ForValidData_LogsIn()
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTestingAsync();
        PasswordService passwordService = new();
        string password = passwordService.HashPassword("Password123!");
        JwtDetails jwtDetails = new()
        {
            SecretKey = "SuperSecretKeyForJwtToken123123123456",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            LifetimeInMinutes = 60
        };
        UserService userService = new(usersDatabaseContext, passwordService, new JwtService(), Options.Create(jwtDetails), new RoleService(usersDatabaseContext), new MultifactorAuthenticationService());
        await usersDatabaseContext.Users.AddAsync(new User() { Password = password, Email = "emailtest@emailtest.pl", Username = "name12" });
        await usersDatabaseContext.SaveChangesAsync();
        LogInDto logInDto = new() { Email = "emailtest@emailtest.pl", Password = "Password123!" };

        //Act
        UserSignedInDto? result = await userService.LogInAsync(logInDto);

        //Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Username.Should().Be("name12");
        result.Jwt.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("emailthatisnotvalid@email.com", "Password123!")] //Valid password but user with given email does not exist in database
    [InlineData("user@email.com", "invalid")] //User with this email exists in database but password is invalid
    public async Task LogInAsync_ForInvalidCredentials_ReturnsNull(string email, string password)
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTestingAsync();
        PasswordService passwordService = new();
        string hashedPassword = passwordService.HashPassword("Password123!");
        UserService userService = new(usersDatabaseContext, passwordService, new JwtService(), Options.Create(new JwtDetails()), new RoleService(usersDatabaseContext), new MultifactorAuthenticationService());
        await usersDatabaseContext.Users.AddAsync(new User() { Password = hashedPassword, Email = "user@email.com", Username = "name12" });
        await usersDatabaseContext.SaveChangesAsync();
        LogInDto logInDto = new() { Email = email, Password = password };

        //Act
        UserSignedInDto? result = await userService.LogInAsync(logInDto);

        //Assert
        result.Should().BeNull();
    }
}