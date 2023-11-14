using FilmForumModels.Dtos.EpisodeDtos;
using FilmForumWebAPI.Controllers;
using FilmForumWebAPI.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Moq;

namespace FilmForumWebAPI.UnitTests.Controllers;

public class EpisodeControllerTests
{
    private readonly EpisodeController _episodeController;

    private readonly Mock<IEpisodeService> _episodeServiceMock = new();

    public EpisodeControllerTests() => _episodeController = new(_episodeServiceMock.Object);

    [Fact]
    public async Task Create_ForValidData_AddsEpisode()
    {
        // Arrange
        CreateEpisodeDto createEpisodeDto = new()
        {
            Title = "New adventure",
            Description = "Group of people alone in the sea",
            EpisodeNumber = 1,
            SeasonNumber = 1,
            Length = 60,
            Year = 2021,
            FilmId = "1",
            DirectorIds = new() { "1", "2" },
            ActorIds = new() { "1", "2", "3" }
        };
        _episodeServiceMock.Setup(x => x.CreateAsync(createEpisodeDto)).Returns(Task.CompletedTask);

        // Act
        IActionResult result = await _episodeController.Create(createEpisodeDto);

        // Assert
        CreatedResult okObjectResult = result.Should().BeOfType<CreatedResult>().Subject;
        okObjectResult.Value.Should().Be(createEpisodeDto);
    }

    [Fact]
    public async Task SearchAll_ForValidData_ReturnsAllFoundEpisodes()
    {
        // Arrange
        List<GetEpisodeDto> episodes = new()
        {
            new(new(){ Id = "1", Title = "New adventure 1", Description = "Great episode", EpisodeNumber = 1, SeasonNumber = 1, Length = 56, Year = 2021, FilmId = "1", DirectorIds = new() { "1", "2" }, ActorIds = new() { "1", "3" } }),
            new(new(){ Id = "2", Title = "New adventure 2", Description = "Short episode", EpisodeNumber = 2, SeasonNumber = 1, Length = 16, Year = 2021, FilmId = "1", DirectorIds = new() { "1", "2" }, ActorIds = new() { "1", "2", } }),
            new(new(){ Id = "3", Title = "New adventure 3", Description = "You will not forget this episode", EpisodeNumber = 3, SeasonNumber = 1, Length = 61, Year = 2021, FilmId = "1", DirectorIds = new() { "1", "2" }, ActorIds = new() { "1", "2", "3" } }),
        };
        _episodeServiceMock.Setup(x => x.SearchAllAsync(It.IsAny<string>())).ReturnsAsync(episodes);

        // Act
        IActionResult result = await _episodeController.SearchAll(It.IsAny<string>());

        // Assert
        OkObjectResult okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okObjectResult.Value.Should().BeEquivalentTo(episodes);
    }

    [Fact]
    public async Task GetAll_ForValidData_ReturnsAllEpisodes()
    {
        // Arrange
        List<GetEpisodeDto> episodes = new()
        {
            new(new(){ Id = "1", Title = "New adventure 1", Description = "Great episode", EpisodeNumber = 1, SeasonNumber = 1, Length = 56, Year = 2021, FilmId = "1", DirectorIds = new() { "1", "2" }, ActorIds = new() { "1", "3" } }),
            new(new(){ Id = "2", Title = "New adventure 2", Description = "Short episode", EpisodeNumber = 2, SeasonNumber = 1, Length = 16, Year = 2021, FilmId = "1", DirectorIds = new() { "1", "2" }, ActorIds = new() { "1", "2", } }),
        };
        _episodeServiceMock.Setup(x => x.SearchAllAsync(It.IsAny<string>())).ReturnsAsync(episodes);

        // Act
        IActionResult result = await _episodeController.SearchAll(It.IsAny<string>());

        // Assert
        OkObjectResult okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okObjectResult.Value.Should().BeEquivalentTo(episodes);
    }

    [Fact]
    public async Task GetDetailedAll_ForValidData_ReturnsAllEpisodes()
    {
        //Arrange
        List<GetDetailedEpisodeDto> episodes = new()
        {
            new(new() { Id = "1", Title = "New adventure 1", Description = "Great episode", EpisodeNumber = 1, SeasonNumber = 1, Length = 56, Year = 2021, FilmId = "1", DirectorIds = new() { "1", "2" }, ActorIds = new() { "1", "3" } }),
            new(new() { Id = "2", Title = "New adventure 2", Description = "Funny episode", EpisodeNumber = 2, SeasonNumber = 3, Length = 16, Year = 2021, FilmId = "1", DirectorIds = new() { "1", "2", "3" }, ActorIds = new() { "1", "2", "3" } })
        };

        _episodeServiceMock.Setup(x => x.GetDetailedAllAsync()).ReturnsAsync(episodes);

        // Act
        IActionResult result = await _episodeController.GetDetailedAll();

        // Assert
        OkObjectResult okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okObjectResult.Value.Should().BeEquivalentTo(episodes);
    }

    [Fact]
    public async Task GetById_ForValidData_ReturnsEpisode()
    {
        // Arrange
        GetEpisodeDto episode = new(new() { Id = "1", Title = "New adventure 1", Description = "Great episode", EpisodeNumber = 1, SeasonNumber = 1, Length = 56, Year = 2021, FilmId = "1", DirectorIds = new() { "1", "2" }, ActorIds = new() { "1", "3" } });
        _episodeServiceMock.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(episode);

        // Act
        IActionResult result = await _episodeController.GetById(It.IsAny<string>());

        // Assert
        OkObjectResult okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okObjectResult.Value.Should().Be(episode);
    }

    [Fact]
    public async Task GetById_ForNonExistingEpisode_ReturnsNotFound()
    {
        // Arrange
        _episodeServiceMock.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(() => null);

        // Act
        IActionResult result = await _episodeController.GetById(It.IsAny<string>());

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetDetailedById_ForValidData_ReturnsEpisode()
    {
        //Arrange
        GetDetailedEpisodeDto episode = new(new() { Id = "1", Title = "New adventure 1", Description = "Great episode", EpisodeNumber = 1, SeasonNumber = 1, Length = 56, Year = 2021, FilmId = "1", DirectorIds = new() { "1", "2" }, ActorIds = new() { "1", "3" } });
        _episodeServiceMock.Setup(x => x.GetDetailedAsync(It.IsAny<string>())).ReturnsAsync(episode);

        //Act
        IActionResult result = await _episodeController.GetDetailedById(It.IsAny<string>());

        //Assert
        OkObjectResult okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okObjectResult.Value.Should().Be(episode);
    }

    [Fact]
    public async Task GetDetailedById_ForNonExistingEpisode_ReturnsNotFound()
    {
        //Arrange
        _episodeServiceMock.Setup(x => x.GetDetailedAsync(It.IsAny<string>())).ReturnsAsync(() => null);

        //Act
        IActionResult result = await _episodeController.GetDetailedById(It.IsAny<string>());

        //Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Update_ForValidData_UpdatesEpisode()
    {
        // Arrange
        CreateEpisodeDto createEpisodeDto = new()
        {
            Title = "New adventure",
            Description = "Group of people alone in the sea",
            EpisodeNumber = 1,
            SeasonNumber = 1,
            Length = 60,
            Year = 2021,
            FilmId = "1",
            DirectorIds = new() { "1", "2" },
            ActorIds = new() { "1", "2", "3" }
        };
        _episodeServiceMock.Setup(x => x.UpdateAsync(It.IsAny<string>(), createEpisodeDto)).ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, It.IsAny<string>()));

        // Act
        IActionResult result = await _episodeController.Update(It.IsAny<string>(), createEpisodeDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Update_ForNonExistingEpisode_ReturnsNotFound()
    {
        // Arrange
        _episodeServiceMock.Setup(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<CreateEpisodeDto>())).ReturnsAsync(new ReplaceOneResult.Acknowledged(0, null, It.IsAny<string>()));

        // Act
        IActionResult result = await _episodeController.Update(It.IsAny<string>(), It.IsAny<CreateEpisodeDto>());

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Remove_DeletesEpisode()
    {
        // Arrange
        _episodeServiceMock.Setup(x => x.RemoveAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

        // Act
        IActionResult result = await _episodeController.Remove(It.IsAny<string>());

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }
}