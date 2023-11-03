using FilmForumModels.Dtos.ActorDtos;
using FilmForumModels.Entities;
using FilmForumWebAPI.Database;
using FilmForumWebAPI.Services.Interfaces;
using MongoDB.Driver;

namespace FilmForumWebAPI.Services;

public class ActorService : IActorService
{
    private readonly IMongoCollection<Actor> _actorCollection;

    public ActorService(FilmsDatabaseContext filmsDatabaseContext)
    {
        _actorCollection = filmsDatabaseContext.ActorCollection;
    }

    public async Task CreateAsync(CreateActorDto createActorDto) => await _actorCollection.InsertOneAsync(new(createActorDto));

    public async Task<List<GetActorDto>> GetAllAsync()
        => await _actorCollection.Find(_ => true).ToListAsync() is IEnumerable<Actor> list ? list.Select(x => new GetActorDto(x)).ToList() : new();

    public async Task<GetActorDto?> GetAsync(string id)
        => await _actorCollection.Find(x => x.Id == id).FirstOrDefaultAsync() is Actor actor ? new(actor) : null;

    public async Task UpdateAsync(string id, CreateActorDto createActorDto)
        => await _actorCollection.ReplaceOneAsync(x => x.Id == id, new(id, createActorDto));

    public async Task RemoveAsync(string id)
        => await _actorCollection.DeleteOneAsync(x => x.Id == id);
}