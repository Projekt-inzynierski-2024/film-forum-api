using FilmForumWebAPI.Services.Interfaces;
using FilmForumWebAPI.Models.Entities;
using Microsoft.Extensions.Options;
using FilmForumWebAPI.Models;
using MongoDB.Driver;

namespace FilmForumWebAPI.Services;

public class FilmService : IFilmService
{
    private readonly IMongoCollection<Film> filmsCollection;

    public FilmService(IOptions<FilmForumMongoDatabaseSettings> mongoDatabaseSettings)
    {
        var mongoClient = new MongoClient(mongoDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(mongoDatabaseSettings.Value.DatabaseName);

        filmsCollection = mongoDatabase.GetCollection<Film>(mongoDatabaseSettings.Value.FilmsCollectionName);
    }

    public async Task<List<Film>> GetAsync()
    {
        return await filmsCollection.Find(_ => true).ToListAsync();
    }

    public async Task<Film?> GetAsync(string id)
    {
        return await filmsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Film newFilm)
    {
        await filmsCollection.InsertOneAsync(newFilm);
    }

    public async Task UpdateAsync(string id, Film updatedFilm)
    {
        await filmsCollection.ReplaceOneAsync(x => x.Id == id, updatedFilm);
    }

    public async Task RemoveAsync(string id)
    {
        await filmsCollection.DeleteOneAsync(x => x.Id == id);
    }
}
