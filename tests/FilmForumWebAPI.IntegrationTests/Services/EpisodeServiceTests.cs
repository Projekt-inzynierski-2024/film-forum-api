using FilmForumModels.Dtos.EpisodeDtos;
using FilmForumModels.Entities;
using FilmForumWebAPI.Database;
using FilmForumWebAPI.IntegrationTests.HelpersForTests;
using FilmForumWebAPI.Services;
using FluentAssertions;
using MongoDB.Driver;

namespace FilmForumWebAPI.IntegrationTests.Services;

public class EpisodeServiceTests
{
    [Fact]
    public async Task SearchAllAsync_ForGivenQuery_ReturnsFoundEpisodes()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        EpisodeService episodeService = new(filmsDatabaseContext);
        List<Episode> episodes = new()
        {
            new Episode { Title = "Episode 1", Description = "Funny movie", Year = 2021, Length = 120, SeasonNumber = 1, EpisodeNumber = 1, FilmId = "123123498731231891721823"},
            new Episode { Title = "Episode 2", Description = "Funny movie", Year = 2022, Length = 120, SeasonNumber = 1, EpisodeNumber = 2, FilmId = "123123498731231891721823"},
            new Episode { Title = "Episode 3", Description = "Funny movie", Year = 2023, Length = 120, SeasonNumber = 1, EpisodeNumber = 3, FilmId = "123123498731231891721823"},
            new Episode { Title = "Episode 4", Description = "Funny movie", Year = 2024, Length = 120, SeasonNumber = 1, EpisodeNumber = 4, FilmId = "123123498731231891721823"},
            new Episode { Title = "Movie 1", Description = "Great movie", Year = 2021, Length = 120, SeasonNumber = 1, EpisodeNumber = 1, FilmId = "123123498731231891721823"},
            new Episode { Title = "Movie 2", Description = "Great movie", Year = 2021, Length = 120, SeasonNumber = 1, EpisodeNumber = 2, FilmId = "123123498731231891721823"},
        };
        await filmsDatabaseContext.EpisodeCollection.InsertManyAsync(episodes);

        // Act
        List<GetEpisodeDto> result = await episodeService.SearchAllAsync("Episode");

        // Assert
        result.Should().HaveCount(4);
    }

    [Fact]
    public async Task CreateAsync_ForGivenData_CreatesEpisode()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        EpisodeService episodeService = new(filmsDatabaseContext);
        CreateEpisodeDto createEpisodeDto = new() { Title = "Amazing movie", Description = "Just watch", Length = 120, FilmId = "123123498731231891721823" };

        // Act
        await episodeService.CreateAsync(createEpisodeDto);

        // Assert
        Episode? createdEpisode = await filmsDatabaseContext.EpisodeCollection.Find(x => x.Title == createEpisodeDto.Title).FirstOrDefaultAsync();
        createdEpisode.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAllAsync_ForExistingEpisodes_ReturnsAllEpisodes()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        EpisodeService episodeService = new(filmsDatabaseContext);
        List<Episode> episodes = new()
        {
            new Episode { Title = "Episode 1", Description = "Funny movie", Year = 2021, Length = 120, SeasonNumber = 1,  EpisodeNumber = 1, FilmId = "123123498731231891721823"},
            new Episode { Title = "Episode 2", Description = "Funny movie", Year = 2022, Length = 120, SeasonNumber = 1,  EpisodeNumber = 2, FilmId = "123123498731231891721823"},
            new Episode { Title = "Episode 3", Description = "Funny movie", Year = 2023, Length = 120, SeasonNumber = 1,  EpisodeNumber = 3 , FilmId = "123123498731231891721823"},
            new Episode { Title = "Episode 4", Description = "Funny movie", Year = 2024, Length = 120, SeasonNumber = 1,  EpisodeNumber = 4, FilmId = "123123498731231891721823"},
            new Episode { Title = "Movie 1", Description = "Great movie", Year = 2021, Length = 120, SeasonNumber = 1,  EpisodeNumber = 1, FilmId = "123123498731231891721823"},
            new Episode { Title = "Movie 2", Description = "Great movie", Year = 2021, Length = 120, SeasonNumber = 1,  EpisodeNumber = 2, FilmId = "123123498731231891721823"},
        };
        await filmsDatabaseContext.EpisodeCollection.InsertManyAsync(episodes);

        // Act
        List<GetEpisodeDto> result = await episodeService.GetAllAsync();

        // Assert
        result.Should().HaveCount(6);
    }

    [Fact]
    public async Task GetDetailedAllAsync_ForExistingEpisodes_ReturnsDetailedAllEpisodes()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        Film film = new() { Description = "Prepare for good fun", Title = "FunnyMovie" };
        await filmsDatabaseContext.FilmCollection.InsertOneAsync(film);
        List<Episode> episodes = new()
        {
            new() { Title = "Episode 1", Description = "Funny movie", FilmId = film.Id },
            new() { Title = "Episode 2", Description = "Funny movie", FilmId = film.Id },
            new() { Title = "Episode 3", Description = "Funny movie", FilmId = film.Id },
            new() { Title = "Episode 4", Description = "Funny movie", FilmId = film.Id },
            new() { Title = "Episode 5", Description = "Funny movie", FilmId = film.Id },
            new() { Title = "Episode 6", Description = "Funny movie", FilmId = film.Id },
        };
        await filmsDatabaseContext.EpisodeCollection.InsertManyAsync(episodes);
        EpisodeService episodeService = new(filmsDatabaseContext);

        // Act
        List<GetDetailedEpisodeDto> result = await episodeService.GetDetailedAllAsync();

        // Assert
        result.Should().HaveCount(6);
    }

    [Fact]
    public async Task GetAsync_ForExistingEpisode_ReturnsEpisode()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        EpisodeService episodeService = new(filmsDatabaseContext);
        Episode episode = new() { Title = "Episode 1", Description = "Funny movie", Year = 2021, Length = 120, SeasonNumber = 1, EpisodeNumber = 1, FilmId = "123123498731231891721823" };
        await filmsDatabaseContext.EpisodeCollection.InsertOneAsync(episode);

        // Act
        GetEpisodeDto? result = await episodeService.GetAsync(episode.Id);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAsync_ForNotExistingEpisode_ReturnsNull()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        EpisodeService episodeService = new(filmsDatabaseContext);

        // Act
        GetEpisodeDto? result = await episodeService.GetAsync("999999999999999999999999");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetDetailedAsync_ForExistingEpisode_ReturnsDetailedEpisode()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        Film film = new() { Description = "Prepare for good fun", Title = "FunnyMovie" };
        await filmsDatabaseContext.FilmCollection.InsertOneAsync(film);
        Episode episode = new() { Title = "Episode 1", Description = "Funny movie", Year = 2021, Length = 120, SeasonNumber = 1, EpisodeNumber = 1, FilmId = film.Id };
        await filmsDatabaseContext.EpisodeCollection.InsertOneAsync(episode);
        EpisodeService episodeService = new(filmsDatabaseContext);

        // Act
        GetDetailedEpisodeDto? result = await episodeService.GetDetailedAsync(episode.Id);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetDetailedAsync_ForNotExistingEpisode_ReturnsNull()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        EpisodeService episodeService = new(filmsDatabaseContext);

        // Act
        GetEpisodeDto? result = await episodeService.GetAsync("999999999999999999999999");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ForExistingEpisode_UpdatesEpisode()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        Film film = new() { Description = "Prepare for good fun", Title = "FunnyMovie" };
        await filmsDatabaseContext.FilmCollection.InsertOneAsync(film);
        Episode episode = new() { Title = "Episode 1", Description = "Funny movie", Year = 2021, Length = 120, SeasonNumber = 1, EpisodeNumber = 1, FilmId = film.Id };
        await filmsDatabaseContext.EpisodeCollection.InsertOneAsync(episode);
        EpisodeService episodeService = new(filmsDatabaseContext);
        CreateEpisodeDto createEpisodeDto = new() { Title = "New title", Description = "New Description", FilmId = film.Id };

        // Act
        ReplaceOneResult result = await episodeService.UpdateAsync(episode.Id, createEpisodeDto);

        // Assert
        result.ModifiedCount.Should().Be(1);
        Episode updatedEpisode = await filmsDatabaseContext.EpisodeCollection.Find(x => x.Id == episode.Id).FirstOrDefaultAsync();
        updatedEpisode.Title.Should().Be(createEpisodeDto.Title);
        updatedEpisode.Description.Should().Be(createEpisodeDto.Description);
    }

    [Fact]
    public async Task UpdateAsync_ForNonExistiningEpisode_DoesNotUpdateEpisode()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        EpisodeService episodeService = new(filmsDatabaseContext);
        CreateEpisodeDto createEpisodeDto = new() { Title = "New title", Description = "New Description", FilmId = "123123498731231891721823" };

        // Act
        ReplaceOneResult result = await episodeService.UpdateAsync("999999999999999999999999", createEpisodeDto);

        // Assert
        result.ModifiedCount.Should().Be(0);
    }

    [Fact]
    public async Task RemoveAsync_ForExistingEpisode_RemovesEpisode()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        EpisodeService episodeService = new(filmsDatabaseContext);
        Film film = new() { Description = "Prepare for good fun", Title = "FunnyMovie" };
        await filmsDatabaseContext.FilmCollection.InsertOneAsync(film);
        Episode episode = new() { Title = "Episode 1", Description = "Funny movie", Year = 2021, Length = 120, SeasonNumber = 1, EpisodeNumber = 1, FilmId = film.Id };
        await filmsDatabaseContext.EpisodeCollection.InsertOneAsync(episode);

        // Act
        DeleteResult result = await episodeService.RemoveAsync(episode.Id);

        // Assert
        result.DeletedCount.Should().Be(1);
    }

    [Fact]
    public async Task RemoveAsync_ForNotExistingEpisode_DoesNotRemoveEpisode()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        EpisodeService episodeService = new(filmsDatabaseContext);

        // Act
        DeleteResult result = await episodeService.RemoveAsync("999999999999999999999999");

        // Assert
        result.DeletedCount.Should().Be(0);
    }
}