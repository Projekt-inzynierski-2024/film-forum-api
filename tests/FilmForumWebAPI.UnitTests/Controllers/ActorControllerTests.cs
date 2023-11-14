using FilmForumModels.Dtos.ActorDtos;
using FilmForumWebAPI.Controllers;
using FilmForumWebAPI.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Moq;

namespace FilmForumWebAPI.UnitTests.Controllers;

public class ActorControllerTests
{
    private readonly ActorController _actorController;

    private readonly Mock<IActorService> _actorServiceMock = new();

    public ActorControllerTests() => _actorController = new(_actorServiceMock.Object);

    [Fact]
    public async Task Create_ForValidData_AddsActor()
    {
        // Arrange
        CreateActorDto createActorDto = new()
        {
            Name = "Michael",
            Surname = "Capitano",
            Description = "Best actor of 2021",
        };
        _actorServiceMock.Setup(x => x.CreateAsync(createActorDto)).Returns(Task.CompletedTask);

        // Act
        IActionResult result = await _actorController.Create(createActorDto);

        // Assert
        CreatedResult createdResult = result.Should().BeOfType<CreatedResult>().Subject;
        createdResult.Should().Be(createdResult);
        createdResult.Location.Should().Be(nameof(_actorController.GetById));
    }

    [Fact]
    public async Task SearchAll_ForValidData_ReturnsAllFoundActors()
    {
        // Arrange
        List<GetActorDto> actors = new()
        {
            new(new(){ Id = "1", Name = "Michael", Surname = "Capitano", Description = "Best actor of 2021" }),
        };
        _actorServiceMock.Setup(x => x.SearchAllAsync(It.IsAny<string>())).ReturnsAsync(actors);

        // Act
        IActionResult result = await _actorController.SearchAll(It.IsAny<string>());

        // Assert
        OkObjectResult okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okObjectResult.Value.Should().BeEquivalentTo(actors);
    }

    [Fact]
    public async Task GetAll_ForValidData_ReturnsAllActors()
    {
        // Arrange
        List<GetActorDto> actors = new()
        {
            new(new(){ Id = "1", Name = "Michael", Surname = "Capitano", Description = "Best actor of 2021" }),
            new(new(){ Id = "2", Name = "John", Surname = "Smith", Description = "Best actor of 2020" }),
        };
        _actorServiceMock.Setup(x => x.GetAllAsync()).ReturnsAsync(actors);

        // Act
        IActionResult result = await _actorController.GetAll();

        // Assert
        OkObjectResult okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okObjectResult.Value.Should().BeEquivalentTo(actors);
    }

    [Fact]
    public async Task GetById_ForValidData_ReturnsActor()
    {
        // Arrange
        GetActorDto actor = new(new() { Id = "1", Name = "Michael", Surname = "Capitano", Description = "Best actor of 2021" });
        _actorServiceMock.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(actor);

        // Act
        IActionResult result = await _actorController.GetById(It.IsAny<string>());

        // Assert
        OkObjectResult okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okObjectResult.Value.Should().Be(actor);
    }

    [Fact]
    public async Task GetById_ForNonExistingActor_ReturnsNotFound()
    {
        // Arrange
        _actorServiceMock.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(() => null);

        // Act
        IActionResult result = await _actorController.GetById(It.IsAny<string>());

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Update_ForValidData_UpdatesActor()
    {
        // Arrange
        CreateActorDto createActorDto = new()
        {
            Name = "Michael",
            Surname = "Capitano",
            Description = "Best actor of 2021",
        };
        _actorServiceMock.Setup(x => x.UpdateAsync(It.IsAny<string>(), createActorDto)).ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, It.IsAny<string>()));

        // Act
        IActionResult result = await _actorController.Update(It.IsAny<string>(), createActorDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Update_ForNonExistingActor_ReturnsNotFound()
    {
        // Arrange
        _actorServiceMock.Setup(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<CreateActorDto>())).ReturnsAsync(new ReplaceOneResult.Acknowledged(0, null, It.IsAny<string>()));

        // Act
        IActionResult result = await _actorController.Update(It.IsAny<string>(), It.IsAny<CreateActorDto>());

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Remove_DeletesActor()
    {
        // Arrange
        _actorServiceMock.Setup(x => x.RemoveAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

        // Act
        IActionResult result = await _actorController.Remove(It.IsAny<string>());

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }
}