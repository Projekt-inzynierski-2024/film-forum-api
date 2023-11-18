using FilmForumModels.Dtos.RoleDtos;
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
        yield return new object[]
        {
            1,
            new List<UserToRole>()
            {
                new() { RoleId = 1 , UserId = 1},
                new() { RoleId = 2 , UserId = 1},
                new() { RoleId = 3 , UserId = 1},
            },
            new List<string>() { "Admin", "Moderator", "User" }
        };
        yield return new object[]
        {
            1,
            new List<UserToRole>()
            {
                new() { RoleId = 2 , UserId = 1},
                new() { RoleId = 3 , UserId = 1},
            },
            new List<string>() { "Moderator", "User" }
        };
        yield return new object[]
        {
            1,
            new List<UserToRole>()
            {
                new() { RoleId = 3 , UserId = 1},
            },
            new List<string>() { "User" }
        };
    }

    [Theory]
    [MemberData(nameof(GetUserRolesNamesAsyncTestsData))]
    public async Task GetUserRolesNamesAsync_ForExistingUser_ReturnsUserRolesNames(int userId, List<UserToRole> userToRoles, List<string> expected)
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
        await usersDatabaseContext.UsersToRoles.AddRangeAsync(userToRoles);
        await usersDatabaseContext.SaveChangesAsync();

        //Act
        List<string> result = await roleService.GetUserRolesNamesAsync(userId);

        //Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetUserRolesNamesAsync_ForNonExistingUser_ReturnsEmptyList()
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTesting();
        RoleService roleService = new(usersDatabaseContext);

        //Act
        List<string> result = await roleService.GetUserRolesNamesAsync(99999);

        //Assert
        result.Should().BeEmpty();
    }

    public static IEnumerable<object[]> GetUserRolesAsyncTestsData()
    {
        yield return new object[]
        {
            1,
            new List<UserToRole>()
            {
                new() { RoleId = 1 , UserId = 1},
                new() { RoleId = 2 , UserId = 1},
                new() { RoleId = 3 , UserId = 1},
            },
            new List<GetUserRoleDto>()
            {
                new(1, "Admin", new DateTime(2023, 11, 19), 1),
                new(2, "Moderator", new DateTime(2023, 11, 19), 1),
                new(3, "User", new DateTime(2023, 11, 19), 1),
            }
        };
        yield return new object[]
        {
            1,
            new List<UserToRole>()
            {
                new() { RoleId = 2 , UserId = 1},
                new() { RoleId = 3 , UserId = 1},
            },
            new List<GetUserRoleDto>()
            {
                new(2, "Moderator", new DateTime(2023, 11, 19), 1),
                new(3, "User", new DateTime(2023, 11, 19), 1),
            }
        };
        yield return new object[]
        {
            1,
            new List<UserToRole>()
            {
                new() { RoleId = 3 , UserId = 1},
            },
            new List<GetUserRoleDto>()
            {
                new(3, "User", new DateTime(2023, 11, 19), 1),
            }
        };
    }

    [Theory]
    [MemberData(nameof(GetUserRolesAsyncTestsData))]
    public async Task GetUserRolesAsync_ForExistingUser_ReturnsUserRolesNames(int userId, List<UserToRole> userToRoles, List<GetUserRoleDto> expected)
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTesting();
        RoleService roleService = new(usersDatabaseContext);
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "dasd321D!@#@!#a", Email = "test@test.com", Username = "name" });
        await usersDatabaseContext.Roles.AddRangeAsync(new List<Role>()
        {
            new() { Name = "Admin", CreatedAt = new DateTime(2023, 11, 19) },
            new() { Name = "Moderator", CreatedAt = new DateTime(2023, 11, 19) },
            new() { Name = "User", CreatedAt = new DateTime(2023, 11, 19) },
        });
        await usersDatabaseContext.UsersToRoles.AddRangeAsync(userToRoles);
        await usersDatabaseContext.SaveChangesAsync();

        //Act
        List<GetUserRoleDto> result = await roleService.GetUserRolesAsync(userId);

        //Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetUserRolesAsync_ForNonExistingUser_ReturnsEmptyList()
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTesting();
        RoleService roleService = new(usersDatabaseContext);

        //Act
        List<GetUserRoleDto> result = await roleService.GetUserRolesAsync(99999);

        //Assert
        result.Should().BeEmpty();
    }
}