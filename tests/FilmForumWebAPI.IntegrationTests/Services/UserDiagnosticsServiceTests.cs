using FilmForumModels.Entities;
using FilmForumWebAPI.Database;
using FilmForumWebAPI.IntegrationTests.HelpersForTests;
using FilmForumWebAPI.Services;
using FluentAssertions;

namespace FilmForumWebAPI.IntegrationTests.Services;

public class UserDiagnosticsServiceTests
{
    [Theory]
    [InlineData(1, 1)]
    [InlineData(99999, 0)]
    public async Task CreateAsync_ForGivenUserId_CreatesUserDiagnosticsIfUserExistsInDatabase(int userId, int expected)
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTesting();
        UserDiagnosticsService userDiagnosticsService = new(usersDatabaseContext);
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "dasd321D!@#@!#a", Email = "test@test.com", Username = "name" });
        await usersDatabaseContext.SaveChangesAsync();

        //Act
        int result = await userDiagnosticsService.CreateAsync(userId);

        //Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("test@test.com", 1)]
    [InlineData("invalidemailthatisnotindatabase@mail.com", 0)]
    public async Task UpdateLastFailedSignInAsync_ForGivenUserEmail_UpdatesIfUserExistsInDatabase(string email, int expected)
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTesting();
        UserDiagnosticsService userDiagnosticsService = new(usersDatabaseContext);
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "dasd321D!@#@!#a", Email = "test@test.com", Username = "name" });
        await usersDatabaseContext.UserDiagnostics.AddAsync(new UserDiagnostics(1));
        await usersDatabaseContext.SaveChangesAsync();

        //Act
        int result = await userDiagnosticsService.UpdateLastFailedSignInAsync(email);

        //Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(99999, 0)]
    public async Task UpdateLastSuccessfullSignInAsync_ForGivenUserId_UpdatesIfUserExistsInDatabase(int userId, int expected)
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTesting();
        UserDiagnosticsService userDiagnosticsService = new(usersDatabaseContext);
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "dasd321D!@#@!#a", Email = "test@test.com", Username = "name" });
        await usersDatabaseContext.UserDiagnostics.AddAsync(new UserDiagnostics(1));
        await usersDatabaseContext.SaveChangesAsync();

        //Act
        int result = await userDiagnosticsService.UpdateLastSuccessfullSignInAsync(userId);

        //Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(99999, 0)]
    public async Task UpdateLastUsernameChangeAsync_ForGivenUserId_UpdatesIfUserExistsInDatabase(int userId, int expected)
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTesting();
        UserDiagnosticsService userDiagnosticsService = new(usersDatabaseContext);
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "dasd321D!@#@!#a", Email = "test@test.com", Username = "name" });
        await usersDatabaseContext.UserDiagnostics.AddAsync(new UserDiagnostics(1));
        await usersDatabaseContext.SaveChangesAsync();

        //Act
        int result = await userDiagnosticsService.UpdateLastUsernameChangeAsync(userId);

        //Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(99999, 0)]
    public async Task UpdateLastEmailChangeAsync_ForGivenUserId_UpdatesIfUserExistsInDatabase(int userId, int expected)
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTesting();
        UserDiagnosticsService userDiagnosticsService = new(usersDatabaseContext);
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "dasd321D!@#@!#a", Email = "test@test.com", Username = "name" });
        await usersDatabaseContext.UserDiagnostics.AddAsync(new UserDiagnostics(1));
        await usersDatabaseContext.SaveChangesAsync();

        //Act
        int result = await userDiagnosticsService.UpdateLastEmailChangeAsync(userId);

        //Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(99999, 0)]
    public async Task UpdateLastPasswordChangeAsync_ForGivenUserId_UpdatesIfUserExistsInDatabase(int userId, int expected)
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTesting();
        UserDiagnosticsService userDiagnosticsService = new(usersDatabaseContext);
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "dasd321D!@#@!#a", Email = "test@test.com", Username = "name" });
        await usersDatabaseContext.UserDiagnostics.AddAsync(new UserDiagnostics(1));
        await usersDatabaseContext.SaveChangesAsync();

        //Act
        int result = await userDiagnosticsService.UpdateLastPasswordChangeAsync(userId);

        //Assert
        result.Should().Be(expected);
    }
}