using FilmForumModels.Models.Enums;
using FilmForumModels.Models.Exceptions;
using FilmForumWebAPI.Database;
using FilmForumWebAPI.Services;
using FilmForumWebAPI.Services.Interfaces;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FilmForumWebAPI.UnitTests.Services;

public class RoleServiceTests
{
    public static IEnumerable<object[]> UserRolesToSaveInDatabase()
    {
        yield return new object[] { UserRole.Admin, new List<UserRole> { UserRole.Admin, UserRole.Moderator, UserRole.User } };
        yield return new object[] { UserRole.Moderator, new List<UserRole> { UserRole.Moderator, UserRole.User } };
        yield return new object[] { UserRole.User, new List<UserRole> { UserRole.User } };
    }

    [Theory]
    [MemberData(nameof(UserRolesToSaveInDatabase))]
    public void PrepareUserRolesToSaveInDatabase_ForGivenMainRole_ReturnsProperRoles(UserRole userMainRole, List<UserRole> expected)
    {
        // Arrange
        DbContextOptions<UsersDatabaseContext> options = new();
        Mock<UsersDatabaseContext> _usersDatabaseContextMock = new(options);
        RoleService _roleService = new(_usersDatabaseContextMock.Object);

        // Act
        List<UserRole> result = _roleService.PrepareUserRolesToSaveInDatabase(userMainRole);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void PrepareUserRolesToSaveInDatabase_ForInvalidRoleName_ThrowsInvalidRoleNameException()
    {
        // Arrange
        DbContextOptions<UsersDatabaseContext> options = new();
        Mock<UsersDatabaseContext> _usersDatabaseContextMock = new(options);
        RoleService _roleService = new(_usersDatabaseContextMock.Object);

        // Act
        Func<List<UserRole>> action = () => _roleService.PrepareUserRolesToSaveInDatabase((UserRole)99999);

        // Assert
        action.Should().Throw<InvalidRoleNameException>();
    }
}