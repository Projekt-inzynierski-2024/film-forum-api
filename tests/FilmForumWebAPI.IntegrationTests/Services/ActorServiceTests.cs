using FilmForumModels.Dtos.ActorDtos;
using FilmForumModels.Entities;
using FilmForumWebAPI.Database;
using FilmForumWebAPI.IntegrationTests.HelpersForTests;
using FilmForumWebAPI.Services;
using FluentAssertions;
using MongoDB.Driver;

namespace FilmForumWebAPI.IntegrationTests.Services;

public class ActorServiceTests
{
    [Fact]
    public async Task SearchAllAsync_ForExistingActors_ReturnsFoundActors()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        ActorService actorService = new(filmsDatabaseContext);
        List<Actor> actors = new()
        {
            new Actor() { Name = "Arek", Surname = "Baran", Description = "Great actor" },
            new Actor() { Name = "Kuba", Surname = "Baran", Description = "So great actor" },
            new Actor() { Name = "Jan", Surname = "Baran", Description = "Amazing actor" },
            new Actor() { Name = "Jan", Surname = "Baran", Description = "Super actor" }
        };
        await filmsDatabaseContext.ActorCollection.InsertManyAsync(actors);

        // Act
        List<GetActorDto> result = await actorService.SearchAllAsync("Jan");

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateAsync_ForGivenData_CreatesActor()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        ActorService actorService = new(filmsDatabaseContext);
        CreateActorDto createActorDto = new() { Name = "Jan", Surname = "Baran", Description = "Great actor" };

        // Act
        await actorService.CreateAsync(createActorDto);

        // Assert
        Actor? result = await filmsDatabaseContext.ActorCollection.Find(x => x.Name == "Jan").FirstOrDefaultAsync();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAllAsync_ForExistingActors_ReturnsAllActors()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        ActorService actorService = new(filmsDatabaseContext);
        List<Actor> actors = new()
        {
            new(){ Name = "Jan", Surname = "Baran", Description = "Great actor" },
            new(){ Name = "Arek", Surname = "Baran", Description = "Great actor" },
            new(){ Name = "Jakub", Surname = "Baran", Description = "Great actor" }
        };
        await filmsDatabaseContext.ActorCollection.InsertManyAsync(actors);

        // Act
        List<GetActorDto> result = await actorService.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAsync_ForExistingActor_ReturnsActor()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        ActorService actorService = new(filmsDatabaseContext);
        Actor actor = new() { Name = "Jan", Surname = "Baran", Description = "Great actor" };
        await filmsDatabaseContext.ActorCollection.InsertOneAsync(actor);

        // Act
        GetActorDto? result = await actorService.GetAsync(actor.Id);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAsync_ForNotExistingActor_ReturnsNull()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        ActorService actorService = new(filmsDatabaseContext);

        // Act
        GetActorDto? result = await actorService.GetAsync("999999999999999999999999");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ForExistingActor_UpdatesActor()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        ActorService actorService = new(filmsDatabaseContext);
        Actor actor = new() { Name = "Jan", Surname = "Baran", Description = "Great actor" };
        await filmsDatabaseContext.ActorCollection.InsertOneAsync(actor);
        CreateActorDto createActorDto = new() { Name = "NewName", Surname = "NewSurname", Description = "NewDescription" };

        // Act
        ReplaceOneResult result = await actorService.UpdateAsync(actor.Id, createActorDto);

        // Assert
        result.ModifiedCount.Should().Be(1);
        Actor updatedActor = await filmsDatabaseContext.ActorCollection.Find(x => x.Id == actor.Id).FirstOrDefaultAsync();
        updatedActor.Name.Should().Be(createActorDto.Name);
        updatedActor.Surname.Should().Be(createActorDto.Surname);
        updatedActor.Description.Should().Be(createActorDto.Description);
    }

    [Fact]
    public async Task UpdateAsync_ForNotExistingActor_DoesNotUpdateActor()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        ActorService actorService = new(filmsDatabaseContext);
        CreateActorDto createActorDto = new() { Name = "NewName", Surname = "NewSurname", Description = "NewDescription" };

        // Act
        ReplaceOneResult result = await actorService.UpdateAsync("999999999999999999999999", createActorDto);

        // Assert
        result.ModifiedCount.Should().Be(0);
    }

    [Fact]
    public async Task RemoveAsync_ForExistingActor_RemovesActor()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        ActorService _actorService = new(filmsDatabaseContext);
        Actor actor = new() { Name = "Jan", Surname = "Baran", Description = "Great actor" };
        await filmsDatabaseContext.ActorCollection.InsertOneAsync(actor);

        // Act
        DeleteResult result = await _actorService.RemoveAsync(actor.Id);

        // Assert
        result.DeletedCount.Should().Be(1);
    }

    [Fact]
    public async Task RemoveAsync_ForNotExistingActor_DoesNotRemoveActor()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        ActorService _actorService = new(filmsDatabaseContext);

        // Act
        DeleteResult result = await _actorService.RemoveAsync("999999999999999999999999");

        // Assert
        result.DeletedCount.Should().Be(0);
    }
}