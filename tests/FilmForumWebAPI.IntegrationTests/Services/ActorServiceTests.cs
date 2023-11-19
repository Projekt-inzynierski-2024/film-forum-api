using FilmForumModels.Dtos.ActorDtos;
using FilmForumModels.Entities;
using FilmForumModels.Models.Enums;
using FilmForumModels.Models.Exceptions;
using FilmForumWebAPI.Database;
using FilmForumWebAPI.IntegrationTests.HelpersForTests;
using FilmForumWebAPI.Services;

namespace FilmForumWebAPI.IntegrationTests.Services;

public class ActorServiceTests
{
    private readonly FilmsDatabaseContext _filmsDatabaseContext;
    private readonly ActorService _actorService;

    public ActorServiceTests()
    {
        _filmsDatabaseContext = new FilmsDatabaseContext();
        _actorService = new ActorService(_filmsDatabaseContext);
    }

    public static IEnumerable<object[]> GetUserRolesNamesAsyncTestsData()
    {
        yield return new object[]
        {
            1,
            new List<Actor>()
            {
                 
                new() { Name = 'Jakub', Surname = 'Czura', Description = 'Programista c kratka' },
                new() { Name = 'Jan', Surname = 'Gil', Description = 'Docker swir'},
                new() { Name = 'Arkadiusz', Surname = 'Jezierski', Description = 'Wsparcie i katering'},
            }
        };
    }
    [Fact]
    public async Task SearchAllAsync_ReturnEmptyList()
    {
        // Arrange
        var nonMatchingQuery = "BrzeczyszczykiewiczGrzegorz";

        // Act
        var result = await _actorService.SearchAllAsync(nonMatchingQuery);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
    [Fact]
    public async Task CreateAsync_CreateNewActor()
    {
        // Arrange
        var actorDto = new CreateActorDto;
        var count = (await _actorService.GetAllAsync()).Count;

        // Act
        await _actorService.CreateAsync(actorDto);

        // Assert
        var actors = await _actorService.GetAllAsync();
        actors.Should().HaveCount(count + 1);
        actors.Last().Name.Should().Be(actorDto.Name);
    }
    [Fact]
    public async Task SearchAllAsync_ReturnMatchingActors()
    {
        // Arrange
        var query = "Jan";

        // Act
        var result = await _actorService.SearchAllAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IEnumerable<GetActorDto>>();
        result.Should().OnlyContain(actor => actor.Name.ToUpper().Contains(query.ToUpper()) || actor.Surname.ToUpper().Contains(query.ToUpper()));
    }


    [Fact]
    public async Task GetAllAsync_ReturnAllActors()
    {
        // Act
        var result = await _actorService.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IEnumerable<GetActorDto>>();
        result.Should().HaveCountGreaterThan(0); 
    }

    [Fact]
    public async Task GetAsync_ReturnActorById()
    {
        // Arrange
        var actors = await _actorService.GetAllAsync();
        var actorId = actors.FirstOrDefault()?.Id; 

        // Act
        var result = await _actorService.GetAsync(actorId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(actorId);
    }

    [Fact]
    public async Task UpdateAsync_UpdateExistingActor()
    {
        // Arrange
        var actors = await _actorService.GetAllAsync();
        var actorToUpdate = actors.FirstOrDefault();
        var updatedActorDto = new CreateActorDto;

        // Act
        var result = await _actorService.UpdateAsync(actorToUpdate.Id, updatedActorDto);

        // Assert
        result.Should().NotBeNull();
        result.IsAcknowledged.Should().BeTrue(); 

        var updatedActor = await _actorService.GetAsync(actorToUpdate.Id);
        updatedActor.Should().NotBeNull();
        updatedActor.Name.Should().Be(updatedActorDto.Name);
    }

    [Fact]
    public async Task RemoveAsync_RemoveActor()
    {
        // Arrange
        var actors = await _actorService.GetAllAsync();
        var actorToRemove = actors.FirstOrDefault();
        var initialCount = actors.Count;

        // Act
        await _actorService.RemoveAsync(actorToRemove.Id);

        // Assert
        var remainingActors = await _actorService.GetAllAsync();
        remainingActors.Should().HaveCount(initialCount - 1);
        remainingActors.Should().NotContain(actor => actor.Id == actorToRemove.Id);
    }

}