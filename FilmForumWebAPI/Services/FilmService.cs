using FilmForumWebAPI.Services.Interfaces;
using FilmForumWebAPI.Models.Entities;
using Microsoft.Extensions.Options;
using FilmForumWebAPI.Models;
using MongoDB.Driver;
using FilmForumWebAPI.Models.Dtos.Film;

namespace FilmForumWebAPI.Services;

public class FilmService : IFilmService
{
    private readonly IMongoCollection<Film> _filmsCollection;

    public FilmService(IOptions<FilmForumMongoDatabaseSettings> mongoDatabaseSettings)
    {
        MongoClient mongoClient = new(mongoDatabaseSettings.Value.ConnectionString);

        IMongoDatabase mongoDatabase = mongoClient.GetDatabase(mongoDatabaseSettings.Value.DatabaseName);

        _filmsCollection = mongoDatabase.GetCollection<Film>(mongoDatabaseSettings.Value.FilmsCollectionName);
    }

    public async Task<List<Film>> GetAllAsync() => await _filmsCollection.Find(_ => true).ToListAsync();

    public async Task<Film?> GetAsync(string id) => await _filmsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(CreateFilmDto createFilmDto) => await _filmsCollection.InsertOneAsync(new(createFilmDto));

    public async Task UpdateAsync(string id, CreateFilmDto updatedFilm) => await _filmsCollection.ReplaceOneAsync(x => x.Id == id, new(updatedFilm));

    public async Task RemoveAsync(string id) => await _filmsCollection.DeleteOneAsync(x => x.Id == id);
}
