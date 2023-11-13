using FilmForumModels.Dtos.RoleDtos;
using FilmForumWebAPI.Controllers;
using FilmForumWebAPI.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FilmForumWebAPI.UnitTests.Controllers;

public class RoleControllerTests
{
    private readonly RoleController _roleController;

    private readonly Mock<IRoleService> _roleServiceMock = new();
    private readonly Mock<IUserService> _userServiceMock = new();

    public RoleControllerTests()
    {
        _roleController = new(_roleServiceMock.Object,
                              _userServiceMock.Object);
    }

    public async Task GetUserRolesNames_ForValidData_ReturnsUserRolesName()
    {
        // Arrange
        List<string> rolesNames = new() { "Admin", "Moderator", "User" };
        _userServiceMock.Setup(x => x.UserWithIdExistsAsync(It.IsAny<int>())).ReturnsAsync(true);
        _roleServiceMock.Setup(x => x.GetUserRolesNamesAsync(It.IsAny<int>())).ReturnsAsync(rolesNames);

        // Act
        IActionResult result = await _roleController.GetUserRolesNames(It.IsAny<int>());

        // Assert
        OkObjectResult okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okObjectResult.Value.Should().BeEquivalentTo(rolesNames);
    }

    public async Task GetUserRolesNames_ForNonExistingUser_ReturnsNotFound()
    {
        // Arrange
        List<string> rolesNames = new() { "Admin", "Moderator", "User" };
        _userServiceMock.Setup(x => x.UserWithIdExistsAsync(It.IsAny<int>())).ReturnsAsync(false);

        // Act
        IActionResult result = await _roleController.GetUserRolesNames(It.IsAny<int>());

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    public async Task GetUserRoles_ForValidData_ReturnsUserRoles()
    {
        // Arrange
        List<GetUserRoleDto> roles = new()
        {
            new(1, "Admin", DateTime.UtcNow, 1),
            new(2, "Moderator", DateTime.UtcNow, 1),
            new(3, "User", DateTime.UtcNow, 1),
        };
        _userServiceMock.Setup(x => x.UserWithIdExistsAsync(It.IsAny<int>())).ReturnsAsync(true);
        _roleServiceMock.Setup(x => x.GetUserRolesAsync(It.IsAny<int>())).ReturnsAsync(roles);

        // Act
        IActionResult result = await _roleController.GetUserRoles(It.IsAny<int>());

        // Assert
        OkObjectResult okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okObjectResult.Value.Should().BeEquivalentTo(roles);
    }

    public async Task GetUserRoles_ForNonExistingUser_ReturnsNotFound()
    {
        // Arrange
        _userServiceMock.Setup(x => x.UserWithIdExistsAsync(It.IsAny<int>())).ReturnsAsync(false);

        // Act
        IActionResult result = await _roleController.GetUserRoles(It.IsAny<int>());

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }
}