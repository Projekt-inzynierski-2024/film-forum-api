using FilmForumModels.Dtos.DirectorDtos;
using FilmForumModels.Entities;
using FilmForumWebAPI.Database;
using FilmForumWebAPI.IntegrationTests.HelpersForTests;
using FilmForumWebAPI.Services;
using FluentAssertions;
using MongoDB.Driver;

namespace FilmForumWebAPI.IntegrationTests.Services;

public class DirectorServiceTests
{
    [Fact]
    public async Task SearchAllAsync_ForExistingDirectors_ReturnsFoundDirectors()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        DirectorService _directorService = new(filmsDatabaseContext);
        List<Director> directors = new()
        {
                new Director() { Name = "Arek", Surname = "Baran", Description = "Great director" },
                new Director() { Name = "Kuba", Surname = "Baran", Description = "So great director" },
                new Director() { Name = "Jan", Surname = "Baran", Description = "Amazing director" },
                new Director() { Name = "Jan", Surname = "Baran", Description = "Super director" }
        };
        await filmsDatabaseContext.DirectorCollection.InsertManyAsync(directors);

        // Act
        List<GetDirectorDto> result = await _directorService.SearchAllAsync("Jan");

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateAsync_ForGivenData_CreatesDirector()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        DirectorService _directorService = new(filmsDatabaseContext);
        CreateDirectorDto createDirectorDto = new() { Name = "Arek", Surname = "Baran", Description = "Great director" };

        // Act
        await _directorService.CreateAsync(createDirectorDto);

        // Assert
        Director? result = await filmsDatabaseContext.DirectorCollection.Find(x => x.Name == "Arek").FirstOrDefaultAsync();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAllAsync_ForExistingDirectors_ReturnsAllDirectors()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        DirectorService _directorService = new(filmsDatabaseContext);
        List<Director> directors = new()
        {
            new Director() { Name = "Arek", Surname = "Baran", Description = "Great director" },
            new Director() { Name = "Kuba", Surname = "Baran", Description = "So great director" },
            new Director() { Name = "Jan", Surname = "Baran", Description = "Amazing director" },
            new Director() { Name = "Jan", Surname = "Baran", Description = "Super director" }
        };
        await filmsDatabaseContext.DirectorCollection.InsertManyAsync(directors);

        // Act
        List<GetDirectorDto> result = await _directorService.GetAllAsync();

        // Assert
        result.Should().HaveCount(4);
    }

    [Fact]
    public async Task GetAsync_ForExistingDirector_ReturnsDirector()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        DirectorService _directorService = new(filmsDatabaseContext);
        Director director = new() { Name = "Arek", Surname = "Baran", Description = "Great director" };
        await filmsDatabaseContext.DirectorCollection.InsertOneAsync(director);

        // Act
        GetDirectorDto? result = await _directorService.GetAsync(director.Id);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAsync_ForNotExistingDirector_DoesNotReturnDirector()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        DirectorService _directorService = new(filmsDatabaseContext);

        // Act
        GetDirectorDto? result = await _directorService.GetAsync("999999999999999999999999");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ForExistingDirector_UpdatesDirector()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        DirectorService _directorService = new(filmsDatabaseContext);
        Director director = new() { Name = "Arek", Surname = "Baran", Description = "Great director" };
        await filmsDatabaseContext.DirectorCollection.InsertOneAsync(director);
        CreateDirectorDto createDirectorDto = new() { Name = "Kuba", Surname = "Baran", Description = "So great director" };

        // Act
        ReplaceOneResult result = await _directorService.UpdateAsync(director.Id, createDirectorDto);

        // Assert
        result.ModifiedCount.Should().Be(1);
        Director updatedDirector = await filmsDatabaseContext.DirectorCollection.Find(x => x.Id == director.Id).FirstOrDefaultAsync();
        updatedDirector.Name.Should().Be(createDirectorDto.Name);
        updatedDirector.Surname.Should().Be(createDirectorDto.Surname);
        updatedDirector.Description.Should().Be(createDirectorDto.Description);
    }


    [Fact]
    public async Task UpdateAsync_ForNotExistingDirector_DoesNotUpdateDirector()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        DirectorService _directorService = new(filmsDatabaseContext);      
        CreateDirectorDto createDirectorDto = new() { Name = "Kuba", Surname = "Baran", Description = "So great director" };

        // Act
        ReplaceOneResult result = await _directorService.UpdateAsync("999999999999999999999999", createDirectorDto);

        // Assert
        result.ModifiedCount.Should().Be(0);
    }

    [Fact]
    public async Task RemoveAsync_ForExistingDirector_RemovesDirector()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        DirectorService _directorService = new(filmsDatabaseContext);
        Director director = new() { Name = "Arek", Surname = "Baran", Description = "Great director" };
        await filmsDatabaseContext.DirectorCollection.InsertOneAsync(director);

        // Act
        DeleteResult result = await _directorService.RemoveAsync(director.Id);

        // Assert
        result.DeletedCount.Should().Be(1);
    }

    [Fact]
    public async Task RemoveAsync_ForNotExistingDirector_DoesNotRemoveDirector()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        DirectorService _directorService = new(filmsDatabaseContext);

        // Act
        DeleteResult result = await _directorService.RemoveAsync("999999999999999999999999");

        // Assert
        result.DeletedCount.Should().Be(0);
    }
}