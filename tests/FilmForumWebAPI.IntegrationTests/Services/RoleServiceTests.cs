using FilmForumModels.Entities;
using FilmForumWebAPI.Database;
using FilmForumWebAPI.IntegrationTests.HelpersForTests;
using FilmForumWebAPI.Services;
using FluentAssertions;

namespace FilmForumWebAPI.IntegrationTests.Services;

public class RoleServiceTests
{
    public static IEnumerable<object[]> GetUserRolesNamesAsyncTestsData()
    {
        yield return new object[] { 1, new List<string>() { "Admin", "Moderator", "User" } };
        yield return new object[] { 99999, new List<string>() };
    }

    [Theory]
    [MemberData(nameof(GetUserRolesNamesAsyncTestsData))]
    public async Task GetUserRolesNamesAsync_ForGivenId_ReturnsUserRolesNamesIfUserExistsOtherwiseReturnsEmptyList(int userId, List<string> expected)
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTesting();
        RoleService roleService = new(usersDatabaseContext);
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "dasd321D!@#@!#a", Email = "test@test.com", Username = "name" });
        await usersDatabaseContext.Roles.AddRangeAsync(new List<Role>()
        {
            new() { Name = "Admin" },
            new() { Name = "Moderator" },
            new() { Name = "User" },
        });
        await usersDatabaseContext.UsersToRoles.AddRangeAsync(new List<UserToRole>()
        {
            new() { RoleId = 1, UserId = 1 },
            new() { RoleId = 2, UserId = 1 },
            new() { RoleId = 3, UserId = 1 },
        });
        await usersDatabaseContext.SaveChangesAsync();

        //Act
        List<string> result = await roleService.GetUserRolesNamesAsync(userId);

        //Assert
        result.Should().BeEquivalentTo(expected);
    }
}