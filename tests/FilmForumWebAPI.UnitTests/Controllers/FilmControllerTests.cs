using FilmForumModels.Dtos.FilmDtos;
using FilmForumModels.Entities;
using FilmForumWebAPI.Controllers;
using FilmForumWebAPI.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Moq;
using System.Security.Claims;

namespace FilmForumWebAPI.UnitTests.Controllers;

public class FilmControllerTests
{
    private readonly FilmController _filmController;

    private readonly Mock<IFilmService> _filmServiceMock = new();

    public FilmControllerTests()
    {
        _filmController = new(_filmServiceMock.Object);
    }

    [Fact]
    public async Task Create_ForValidData_AddsFilm()
    {
        // Arrange
        CreateFilmDto createFilmDto = new()
        {
            Title = "Funny movie",
            Description = "A funny family comedy about and old marriage",
            IsMovie = true
        };
        _filmServiceMock.Setup(x => x.CreateAsync(createFilmDto)).Returns(Task.CompletedTask);

        // Act
        IActionResult result = await _filmController.Create(createFilmDto);

        // Assert
        CreatedResult createdResult = result.Should().BeOfType<CreatedResult>().Subject;
        createdResult.Value.Should().Be(createFilmDto);
    }

    [Fact]
    public async Task GetAll_ForValidData_ReturnsAllFilms()
    {
        // Arrange
        List<GetFilmDto> films = new()
        {
            new(new Film { Id = "1" , Title = "Funny movie", Description = "A funny family comedy about and old marriage", IsMovie = true }),
            new(new Film { Id = "2" , Title = "Very funny movie", Description = "Grown ups", IsMovie = true }),
        };
        _filmServiceMock.Setup(x => x.GetAllAsync()).ReturnsAsync(films);

        // Act
        IActionResult result = await _filmController.GetAll();

        // Assert
        OkObjectResult okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okObjectResult.Value.Should().BeEquivalentTo(films);
    }

    [Fact]
    public async Task SearchAll_ForValidData_ReturnsAllFilms()
    {
        // Arrange
        List<GetFilmDto> films = new()
        {
            new(new Film { Id = "1" , Title = "Adventure movie", Description = "Best film of 2024", IsMovie = true }),
        };
        _filmServiceMock.Setup(x => x.SearchAllAsync(It.IsAny<string>())).ReturnsAsync(films);

        // Act
        IActionResult result = await _filmController.SearchAll(It.IsAny<string>());

        // Assert
        OkObjectResult okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okObjectResult.Value.Should().BeEquivalentTo(films);
    }

    [Fact]
    public async Task GetDetailedAll_ForValidData_ReturnsAllFilms()
    {
        // Arrange
        List<GetDetailedFilmDto> films = new()
        {
            new(new Film { Id = "6" , Title = "Great wedding", Description = "Best romantic film of 2023", IsMovie = true }),
        };
        _filmServiceMock.Setup(x => x.GetDetailedAllAsync()).ReturnsAsync(films);

        // Act
        IActionResult result = await _filmController.GetDetailedAll();

        // Assert
        OkObjectResult okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okObjectResult.Value.Should().BeEquivalentTo(films);
    }

    [Fact]
    public async Task GetById_ForValidData_ReturnsFilm()
    {
        // Arrange
        GetFilmDto film = new(new Film { Id = "1", Title = "Funny movie", Description = "A funny family comedy about and old marriage", IsMovie = true });
        _filmServiceMock.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(film);

        // Act
        IActionResult result = await _filmController.GetById(It.IsAny<string>());

        // Assert
        OkObjectResult okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okObjectResult.Value.Should().Be(film);
    }

    [Fact]
    public async Task GetById_ForNonExistingFilm_ReturnsNotFound()
    {
        // Arrange
        _filmServiceMock.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(() => null);

        // Act
        IActionResult result = await _filmController.GetById(It.IsAny<string>());

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetDetailedById_ForValidData_ReturnsFilm()
    {
        // Arrange
        GetDetailedFilmDto film = new(new Film { Id = "1", Title = "Funny movie", Description = "A funny family comedy about and old marriage", IsMovie = true });
        _filmServiceMock.Setup(x => x.GetDetailedAsync(It.IsAny<string>())).ReturnsAsync(film);

        // Act
        IActionResult result = await _filmController.GetDetailedById(It.IsAny<string>());

        // Assert
        OkObjectResult okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okObjectResult.Value.Should().Be(film);
    }

    [Fact]
    public async Task GetDetailedById_ForNonExistingFilm_ReturnsNotFound()
    {
        // Arrange
        _filmServiceMock.Setup(x => x.GetDetailedAsync(It.IsAny<string>())).ReturnsAsync(() => null);

        // Act
        IActionResult result = await _filmController.GetDetailedById(It.IsAny<string>());

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Update_ForValidData_UpdatesFilm()
    {
        // Arrange
        _filmController.ControllerContext.HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "1") }))
        };
        CreateFilmDto createFilmDto = new()
        {
            Title = "Funny movie",
            Description = "A funny family comedy about and old marriage",
            IsMovie = true
        };
        _filmServiceMock.Setup(x => x.UpdateAsync(It.IsAny<string>(), createFilmDto)).ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, It.IsAny<string>()));

        // Act
        IActionResult result = await _filmController.Update(It.IsAny<string>(), createFilmDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Update_ForNonExistingFilm_ReturnsNotFound()
    {
        // Arrange
        _filmController.ControllerContext.HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "1") }))
        };
        _filmServiceMock.Setup(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<CreateFilmDto>())).ReturnsAsync(new ReplaceOneResult.Acknowledged(0, null, It.IsAny<string>()));

        // Act
        IActionResult result = await _filmController.Update(It.IsAny<string>(), It.IsAny<CreateFilmDto>());

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Remove_DeletesFilm()
    {
        // Arrange
        _filmServiceMock.Setup(x => x.RemoveAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

        // Act
        IActionResult result = await _filmController.Remove(It.IsAny<string>());

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }
}