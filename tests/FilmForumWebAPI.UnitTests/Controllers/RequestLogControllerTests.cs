using FilmForumModels.Dtos.RequestDtos;
using FilmForumModels.Entities;
using FilmForumWebAPI.Controllers;
using FilmForumWebAPI.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FilmForumWebAPI.UnitTests.Controllers;

public class RequestLogControllerTests
{
    private readonly RequestLogController _requestLogController;

    private readonly Mock<IRequestLogService> _requestLogServiceMock = new();
    private readonly Mock<IUserService> _userServiceMock = new();

    public RequestLogControllerTests() => _requestLogController = new(_requestLogServiceMock.Object,
                                    _userServiceMock.Object);

    [Fact]
    public async Task GetAll_ForValidData_ReturnsAllRequestLogs()
    {
        // Arrange
        List<GetRequestLogDto> requestLogs = new()
        {
            new(new RequestLog(new(1, "api/user", "192.168.2.142", "POST", 200))),
            new(new RequestLog(new(64, "api/user/80", "192.168.1.162", "GET", 400))),
            new(new RequestLog(new(132, "api/user", "192.168.0.12", "PUT", 204))),
        };
        _requestLogServiceMock.Setup(x => x.GetAllAsync()).ReturnsAsync(requestLogs);

        // Act
        IActionResult result = await _requestLogController.GetAll();

        // Assert
        OkObjectResult okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okObjectResult.Value.Should().BeEquivalentTo(requestLogs);
    }

    [Fact]
    public async Task Get_ForValidData_ReturnsRequestLog()
    {
        // Arrange
        GetRequestLogDto requestLog = new(new RequestLog(new(1, "api/user", "192.168.2.142", "POST", 200)));
        _requestLogServiceMock.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(requestLog);

        //Act
        IActionResult result = await _requestLogController.Get(It.IsAny<int>());

        // Assert
        OkObjectResult okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okObjectResult.Value.Should().Be(requestLog);
    }

    [Fact]
    public async Task Get_ForNonExistingRequestLog_ReturnsNotFound()
    {
        // Arrange
        _requestLogServiceMock.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(() => null);

        //Act
        IActionResult result = await _requestLogController.Get(It.IsAny<int>());

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetUserAllRequestsLogs_ForValidData_ReturnsUserAllRequestLogs()
    {
        // Arrange
        List<GetRequestLogDto> requestLogs = new()
        {
            new(new RequestLog(new(1, "api/user", "192.168.2.142", "POST", 200))),
            new(new RequestLog(new(1, "api/user/80", "192.168.1.162", "GET", 400))),
            new(new RequestLog(new(1, "api/user", "192.168.0.12", "PUT", 204))),
        };
        _userServiceMock.Setup(x => x.UserWithIdExistsAsync(It.IsAny<int>())).ReturnsAsync(true);
        _requestLogServiceMock.Setup(x => x.GetUserAllRequestsLogsAsync(It.IsAny<int>())).ReturnsAsync(requestLogs);

        // Act
        IActionResult result = await _requestLogController.GetUserAllRequestsLogs(It.IsAny<int>());

        // Assert
        OkObjectResult okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okObjectResult.Value.Should().BeEquivalentTo(requestLogs);
    }

    [Fact]
    public async Task GetUserAllRequestsLogs_ForNonExistingUser_ReturnsBadRequest()
    {
        // Arrange
        _userServiceMock.Setup(x => x.UserWithIdExistsAsync(It.IsAny<int>())).ReturnsAsync(false);

        // Act
        IActionResult result = await _requestLogController.GetUserAllRequestsLogs(It.IsAny<int>());

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Remove_DeletesLog()
    {
        // Arrange
        _requestLogServiceMock.Setup(x => x.RemoveAsync(It.IsAny<int>())).ReturnsAsync(1);

        // Act
        IActionResult result = await _requestLogController.Remove(It.IsAny<int>());

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task RemoveUserRequestsLogs_ForValidData_DeletesUserAllRequestLogs()
    {
        // Arrange
        _userServiceMock.Setup(x => x.UserWithIdExistsAsync(It.IsAny<int>())).ReturnsAsync(true);

        // Act
        IActionResult result = await _requestLogController.RemoveUserRequestsLogs(It.IsAny<int>());

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task RemoveUserRequestsLogs_ForNonExistingUser_ReturnsNotFound()
    {
        // Arrange
        _userServiceMock.Setup(x => x.UserWithIdExistsAsync(It.IsAny<int>())).ReturnsAsync(false);

        // Act
        IActionResult result = await _requestLogController.RemoveUserRequestsLogs(It.IsAny<int>());

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }
}