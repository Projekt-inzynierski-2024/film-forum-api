using FilmForumModels.Dtos.RequestDtos;
using FilmForumModels.Entities;
using FilmForumWebAPI.Database;
using FilmForumWebAPI.IntegrationTests.HelpersForTests;
using FilmForumWebAPI.Services;
using FluentAssertions;

namespace FilmForumWebAPI.IntegrationTests.Services;

public class RequestLogServiceTests
{
    [Fact]
    public async Task CreateAsync_ForGivenData_CreatesRequestLog()
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTestingAsync();
        RequestLogService requestLogService = new(usersDatabaseContext);
        CreateRequestLogDto createRequestLogDto = new(1, "api/user", "192.123.2.63", "GET", 200);

        // Act
        int result = await requestLogService.CreateAsync(createRequestLogDto);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task GetAllAsync_ForExistingLogs_ReturnsRequestsLogs()
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTestingAsync();
        RequestLogService requestLogService = new(usersDatabaseContext);
        await usersDatabaseContext.RequestLogs.AddRangeAsync(new List<RequestLog>()
        {
            new(){ UserId = 1, RequestPath = "api/user", IpAddress = "192.123.2.63", HttpMethod = "GET", StatusCode = 200 },
            new(){ UserId = 1, RequestPath = "api/user", IpAddress = "192.123.1.64", HttpMethod = "POST", StatusCode = 400 },
            new(){ UserId = 1, RequestPath = "api/user", IpAddress = "192.123.6.62", HttpMethod = "PUT", StatusCode = 204 }
        });
        await usersDatabaseContext.SaveChangesAsync();

        // Act
        List<GetRequestLogDto> result = await requestLogService.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAsync_ForExistingLogs_ReturnsRequestLog()
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTestingAsync();
        RequestLogService requestLogService = new(usersDatabaseContext);
        await usersDatabaseContext.RequestLogs.AddAsync(new() { UserId = 1, RequestPath = "api/user", IpAddress = "192.168.2.14", HttpMethod = "GET", StatusCode = 200 });
        await usersDatabaseContext.SaveChangesAsync();

        // Act
        GetRequestLogDto? result = await requestLogService.GetAsync(1);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAsync_ForNonExistingRequestLog_ReturnsNull()
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTestingAsync();
        RequestLogService requestLogService = new(usersDatabaseContext);

        // Act
        GetRequestLogDto? result = await requestLogService.GetAsync(999999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUserAllRequestsLogsAsync_ForExistingLogs_ReturnsRequestsLogs()
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTestingAsync();
        RequestLogService requestLogService = new(usersDatabaseContext);
        await usersDatabaseContext.RequestLogs.AddRangeAsync(new List<RequestLog>()
        {
            new(){ UserId = 1, RequestPath = "api/user", IpAddress = "192.123.2.63", HttpMethod = "GET", StatusCode = 200 },
            new(){ UserId = 1, RequestPath = "api/user", IpAddress = "192.123.1.64", HttpMethod = "POST", StatusCode = 400 },
            new(){ UserId = 1, RequestPath = "api/user", IpAddress = "192.123.6.62", HttpMethod = "PUT", StatusCode = 204 },
            new(){ UserId = 9, RequestPath = "api/user", IpAddress = "192.123.6.67", HttpMethod = "PUT", StatusCode = 204 },
            new(){ UserId = 10, RequestPath = "api/user", IpAddress = "192.123.6.68", HttpMethod = "PUT", StatusCode = 204 }
        });
        await usersDatabaseContext.SaveChangesAsync();

        // Act
        List<GetRequestLogDto> result = await requestLogService.GetUserAllRequestsLogsAsync(1);

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task RemoveAsync_ForExistingRequestLog_RemovesRequestLog()
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTestingAsync();
        RequestLogService requestLogService = new(usersDatabaseContext);
        await usersDatabaseContext.RequestLogs.AddAsync(new() { UserId = 1, RequestPath = "api/user", IpAddress = "192.168.2.14", HttpMethod = "GET", StatusCode = 200 });
        await usersDatabaseContext.SaveChangesAsync();

        // Act
        int result = await requestLogService.RemoveAsync(1);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task RemoveAsync_ForNonExistingRequestLog_DoesNotRemoveRequestLog()
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTestingAsync();
        RequestLogService requestLogService = new(usersDatabaseContext);

        // Act
        int result = await requestLogService.RemoveAsync(1);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task RemoveUserRequestsLogsAsync_ForExistingRequestLogs_RemovesRequestLogs()
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTestingAsync();
        RequestLogService requestLogService = new(usersDatabaseContext);
        await usersDatabaseContext.RequestLogs.AddRangeAsync(new List<RequestLog>()
        {
            new(){ UserId = 1, RequestPath = "api/user", IpAddress = "192.168.2.21", HttpMethod = "GET", StatusCode = 200},
            new(){ UserId = 1, RequestPath = "api/user", IpAddress = "192.168.2.21", HttpMethod = "GET", StatusCode = 200},
            new(){ UserId = 1, RequestPath = "api/user", IpAddress = "192.168.2.21", HttpMethod = "GET", StatusCode = 200}
        });
        await usersDatabaseContext.SaveChangesAsync();
       
        //Act
        int result = await requestLogService.RemoveUserRequestsLogsAsync(1);

        //Assert
        result.Should().Be(3);
    }

    [Fact]
    public async Task RemoveUserRequestsLogsAsync_ForNonExistingRequestLogs_DoesNotRemoveRequestLogs()
    {
        //Arrange
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTestingAsync();
        RequestLogService requestLogService = new(usersDatabaseContext);

        //Act
        int result = await requestLogService.RemoveUserRequestsLogsAsync(1);

        //Assert
        result.Should().Be(0);
    }
}