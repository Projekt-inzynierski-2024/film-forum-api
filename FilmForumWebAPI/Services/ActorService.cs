using FilmForumWebAPI.Models;
using FilmForumWebAPI.Models.Dtos.Actor;
using FilmForumWebAPI.Models.Entities;
using FilmForumWebAPI.Services.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FilmForumWebAPI.Services;

public class ActorService : IActorService
{
    private readonly IMongoCollection<Actor> _actorCollection;

    public ActorService(IOptions<FilmForumMongoDatabaseSettings> mongoDatabaseSettings)
    {
        MongoClient mongoClient = new(mongoDatabaseSettings.Value.ConnectionString);

        IMongoDatabase mongoDatabase = mongoClient.GetDatabase(mongoDatabaseSettings.Value.DatabaseName);

        _actorCollection = mongoDatabase.GetCollection<Actor>(mongoDatabaseSettings.Value.ActorsCollectionName);
    }

    public async Task<int> CreateAsync(CreateActorDto createActorDto)
    {
        throw new NotImplementedException();
    }

    public async Task<GetActorDto?> GetAsync(string id)
        => await _actorCollection.Find(x => x.Id == id).FirstOrDefaultAsync() is Actor actor ? new(actor) : null;

    public async Task<List<GetActorDto>> GetAllAsync()
        => await _actorCollection.Find(_ => true).Project(x => new GetActorDto(x)).ToListAsync();
}