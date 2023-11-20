using FilmForumModels.Dtos.FilmDtos;
using FilmForumModels.Entities;
using FilmForumWebAPI.Database;
using FilmForumWebAPI.IntegrationTests.HelpersForTests;
using FilmForumWebAPI.Services;
using FilmForumWebAPI.Services.Interfaces;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FilmForumWebAPI.IntegrationTests.Services;

public class FilmServiceTests
{
    [Fact]
    public async Task SearchAllAsync_ForGivenQuery_ReturnsFoundFilms()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        FilmService filmService = new(filmsDatabaseContext);
        await filmsDatabaseContext.FilmCollection.InsertManyAsync(new List<Film>()
        {
            new() { Title = "FunnyMovie", Description = "Prepare for good fun" },
            new() { Title = "FunnyMovie 2", Description = "Prepare for good fun" },
            new() { Title = "FunnyMovie 3", Description = "Prepare for good fun" },
            new() { Title = "Another movie", Description = "Prepare for good fun" },
            new() { Title = "Another movie 2", Description = "Prepare for good fun" },
        });

        // Act
        List<GetFilmDto> result = await filmService.SearchAllAsync("FunnyMovie");

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAsync_ForExistingFilm_ReturnsFilm()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        FilmService filmService = new(filmsDatabaseContext);
        Film film = new() { Title = "FunnyMovie", Description = "Prepare for good fun" };
        await filmsDatabaseContext.FilmCollection.InsertOneAsync(film);

        // Act
        GetFilmDto? result = await filmService.GetAsync(film.Id);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAsync_ForNonExistingFilm_ReturnsNull()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        FilmService filmService = new(filmsDatabaseContext);

        // Act
        GetFilmDto? result = await filmService.GetAsync("653cb4c0810cec27c0943b01");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetDetailedAsync_ForExistingFilm_ReturnsDetailedFilm()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        Film film = new()
        {
            Title = "FunnyMovie",
            Description = "Prepare for good fun",
            IsMovie = false,
        };
        await filmsDatabaseContext.FilmCollection.InsertOneAsync(film);
        FilmService filmService = new(filmsDatabaseContext);

        // Act
        GetDetailedFilmDto? result = await filmService.GetDetailedAsync(film.Id);
        
        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetDetailedAsync_ForNonExistingFilm_ReturnsNull()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        FilmService filmService = new(filmsDatabaseContext);

        // Act
        GetDetailedFilmDto? result = await filmService.GetDetailedAsync("99999999999999999999999");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ForExistingFilms_ReturnsFilms()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        FilmService filmService = new(filmsDatabaseContext);
        await filmsDatabaseContext.FilmCollection.InsertManyAsync(new List<Film>()
        {
            new() { Title = "FunnyMovie", Description = "Prepare for good fun" },
            new() { Title = "FunnyMovie 2", Description = "Prepare for good fun" },
            new() { Title = "FunnyMovie 3", Description = "Prepare for good fun" },
            new() { Title = "Another movie", Description = "Prepare for good fun" },
            new() { Title = "Another movie 2", Description = "Prepare for good fun" },
        });

        // Act
        List<GetFilmDto> result = await filmService.GetAllAsync();

        // Assert
        result.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetDetailedAllAsync_ForExistingFilms_ReturnsFilms()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        await filmsDatabaseContext.FilmCollection.InsertManyAsync(new List<Film>()
        {
            new() { Title = "FunnyMovie", Description = "Prepare for good fun" },
            new() { Title = "FunnyMovie 2", Description = "Prepare for good fun" },
            new() { Title = "FunnyMovie 3", Description = "Prepare for good fun" },
            new() { Title = "Another movie", Description = "Prepare for good fun" },
            new() { Title = "Another movie 2", Description = "Prepare for good fun" },
        });
        FilmService filmService = new(filmsDatabaseContext);

        // Act
        List<GetDetailedFilmDto> result = await filmService.GetDetailedAllAsync();

        // Assert
        result.Should().HaveCount(5);
    }
}