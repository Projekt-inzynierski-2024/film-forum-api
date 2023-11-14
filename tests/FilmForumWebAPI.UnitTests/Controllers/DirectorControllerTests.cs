using FilmForumModels.Dtos.DirectorDtos;
using FilmForumWebAPI.Controllers;
using FilmForumWebAPI.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Moq;

namespace FilmForumWebAPI.UnitTests.Controllers;

public class DirectorControllerTests
{
    private readonly DirectorController _directorController;

    private readonly Mock<IDirectorService> _directorServiceMock = new();

    public DirectorControllerTests() => _directorController = new DirectorController(_directorServiceMock.Object);

    [Fact]
    public async Task Create_ForValidData_AddsDirector()
    {
        // Arrange
        CreateDirectorDto createDirectorDto = new()
        {
            Name = "Antonio",
            Surname = "Banderas",
            Description = "Best director of 2023",
        };
        _directorServiceMock.Setup(x => x.CreateAsync(createDirectorDto)).Returns(Task.CompletedTask);

        // Act
        IActionResult result = await _directorController.Create(createDirectorDto);

        // Assert
        CreatedResult createdResult = result.Should().BeOfType<CreatedResult>().Subject;
        createdResult.Should().Be(createdResult);
        createdResult.Location.Should().Be(nameof(_directorController.GetById));
    }

    [Fact]
    public async Task SearchAll_ForValidData_ReturnsAllFoundDirectors()
    {
        // Arrange
        List<GetDirectorDto> directors = new()
        {
            new(new() { Id = "1", Name = "Antonio", Surname = "Banderas", Description = "Best director of 2023" }),
        };
        _directorServiceMock.Setup(x => x.SearchAllAsync(It.IsAny<string>())).ReturnsAsync(directors);

        // Act
        IActionResult result = await _directorController.SearchAll(It.IsAny<string>());

        // Assert
        OkObjectResult okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okObjectResult.Value.Should().BeEquivalentTo(directors);
    }

    [Fact]
    public async Task GetAll_ForValidData_ReturnsAllDirectors()
    {
        // Arrange
        List<GetDirectorDto> directors = new()
        {
            new(new() { Id = "1", Name = "Antonio", Surname = "Banderas", Description = "Best director of 2023" }),
            new(new() { Id = "2", Name = "Michael", Surname = "Antonio", Description = "Best director of 2022" }),
            new(new() { Id = "3", Name = "Jakub", Surname = "Bueanaverte", Description = "Best director of 2021" }),
        };
        _directorServiceMock.Setup(x => x.GetAllAsync()).ReturnsAsync(directors);

        // Act
        IActionResult result = await _directorController.GetAll();

        // Assert
        OkObjectResult okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okObjectResult.Value.Should().BeEquivalentTo(directors);
    }

    [Fact]
    public async Task GetById_ForValidData_ReturnsDirector()
    {
        // Arrange
        GetDirectorDto director = new(new() { Id = "1", Name = "Antonio", Surname = "Banderas", Description = "Best director of 2023" });
        _directorServiceMock.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(director);

        // Act
        IActionResult result = await _directorController.GetById(It.IsAny<string>());

        // Assert
        OkObjectResult okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okObjectResult.Value.Should().Be(director);
    }

    [Fact]
    public async Task GetById_ForNonExistingDirector_ReturnsNotFound()
    {
        // Arrange
        _directorServiceMock.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(() => null);

        // Act
        IActionResult result = await _directorController.GetById(It.IsAny<string>());

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Update_ForValidData_UpdatesDirector()
    {
        // Arrange
        CreateDirectorDto createDirectorDto = new()
        {
            Name = "Antonio",
            Surname = "Banderas",
            Description = "Best director of 2023",
        };
        _directorServiceMock.Setup(x => x.UpdateAsync(It.IsAny<string>(), createDirectorDto)).ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, It.IsAny<string>()));

        // Act
        IActionResult result = await _directorController.Update(It.IsAny<string>(), createDirectorDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Update_ForNonExistingDirector_ReturnsNotFound()
    {
        // Arrange
        _directorServiceMock.Setup(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<CreateDirectorDto>())).ReturnsAsync(new ReplaceOneResult.Acknowledged(0, null, It.IsAny<string>()));

        // Act
        IActionResult result = await _directorController.Update(It.IsAny<string>(), It.IsAny<CreateDirectorDto>());

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Remove_DeletesDirector()
    {
        // Arrange
        _directorServiceMock.Setup(x => x.RemoveAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

        // Act
        IActionResult result = await _directorController.Remove(It.IsAny<string>());

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }
}