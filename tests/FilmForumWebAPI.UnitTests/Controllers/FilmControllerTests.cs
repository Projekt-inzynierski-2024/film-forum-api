using FilmForumModels.Dtos.FilmDtos;
using FilmForumModels.Entities;
using FilmForumWebAPI.Controllers;
using FilmForumWebAPI.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
