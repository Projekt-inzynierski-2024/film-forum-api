using FilmForumWebAPI.Models.Dtos.ActorDtos;
using FilmForumWebAPI.Models.Entities;
using FilmForumWebAPI.Models;
using FilmForumWebAPI.Services.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using FilmForumWebAPI.Models.Dtos.DirectorDtos;

namespace FilmForumWebAPI.Services;

public class DirectorService : IDirectorService
{
    private readonly IMongoCollection<Director> _directorCollection;

    public DirectorService(IOptions<FilmForumMongoDatabaseSettings> mongoDatabaseSettings)
    {
        MongoClient mongoClient = new(mongoDatabaseSettings.Value.ConnectionString);

        IMongoDatabase mongoDatabase = mongoClient.GetDatabase(mongoDatabaseSettings.Value.DatabaseName);

        _directorCollection = mongoDatabase.GetCollection<Director>(mongoDatabaseSettings.Value.ActorsCollectionName);
    }
    public async Task CreateAsync(CreateDirectorDto createDirectorDto) => await _directorCollection.InsertOneAsync(new(createDirectorDto));

    public async Task<List<GetDirectorDto>> GetAllAsync() => 
        await _directorCollection.Find(_ => true).ToListAsync() is IEnumerable<Director> directors ? directors.Select(x => new GetDirectorDto(x)).ToList() : new();

    public async Task<GetDirectorDto?> GetAsync(string id)
        => await _directorCollection.Find(x => x.Id == id).FirstOrDefaultAsync() is Director director ? new(director) : null;

    public async Task UpdateAsync(string id, CreateDirectorDto createDirectorDto)
        => await _directorCollection.ReplaceOneAsync(x => x.Id == id, new(createDirectorDto));

    public async Task RemoveAsync(string id)
        => await _directorCollection.DeleteOneAsync(x => x.Id == id);
}
