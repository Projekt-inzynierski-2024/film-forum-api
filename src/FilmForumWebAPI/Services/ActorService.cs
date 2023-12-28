using FilmForumModels.Dtos.ActorDtos;
using FilmForumModels.Entities;
using FilmForumWebAPI.Database;
using FilmForumWebAPI.Services.Interfaces;
using MongoDB.Driver;

namespace FilmForumWebAPI.Services;

public class ActorService : IActorService
{
    private readonly IMongoCollection<Actor> _actorCollection;

    public ActorService(FilmsDatabaseContext filmsDatabaseContext) => _actorCollection = filmsDatabaseContext.ActorCollection;

    /// <summary>
    /// Returns list of actors whose name or surname matches <paramref name="query"/> from database
    /// </summary>
    /// <param name="query">Actor's name or surname</param>
    /// <returns>List of actors with matchind name or surname</returns>
    public async Task<List<GetActorDto>> SearchAllAsync(string query)
        => await _actorCollection.Find(x => (x.Name + " " + x.Surname).ToUpper().Contains(query.ToUpper())).ToListAsync() is IEnumerable<Actor> list ? list.Select(x => new GetActorDto(x)).ToList() : new();

    /// <summary>
    /// Adds new actor to database
    /// </summary>
    /// <param name="createActorDto">Details to create actor</param>
    /// <returns>The result of the insert operation</returns>
    public async Task CreateAsync(CreateActorDto createActorDto) => await _actorCollection.InsertOneAsync(new(createActorDto));

    /// <summary>
    /// Returns list of all actors from database
    /// </summary>
    /// <returns>List of all actors</returns>
    public async Task<List<GetActorDto>> GetAllAsync()
        => await _actorCollection.Find(_ => true).ToListAsync() is IEnumerable<Actor> list ? list.Select(x => new GetActorDto(x)).ToList() : new();

    public async Task<GetActorDto?> GetAsync(string id)
        => await _actorCollection.Find(x => x.Id == id).FirstOrDefaultAsync() is Actor actor ? new(actor) : null;

    public async Task<ReplaceOneResult> UpdateAsync(string id, CreateActorDto createActorDto)
        => await _actorCollection.ReplaceOneAsync(x => x.Id == id, new(id, createActorDto));

    public async Task<DeleteResult> RemoveAsync(string id)
        => await _actorCollection.DeleteOneAsync(x => x.Id == id);
}