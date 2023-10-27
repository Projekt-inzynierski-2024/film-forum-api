using FilmForumWebAPI.Services.Interfaces;
using FilmForumWebAPI.Models.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using FilmForumWebAPI.Models.Dtos.Film;
using FilmForumWebAPI.Models;

namespace FilmForumWebAPI.Services;

public class FilmService : IFilmService
{
    private readonly IMongoCollection<Film> _filmCollection;

    public FilmService(IOptions<FilmForumMongoDatabaseSettings> mongoDatabaseSettings)
    {
        MongoClient mongoClient = new(mongoDatabaseSettings.Value.ConnectionString);

        IMongoDatabase mongoDatabase = mongoClient.GetDatabase(mongoDatabaseSettings.Value.DatabaseName);

        _filmCollection = mongoDatabase.GetCollection<Film>(mongoDatabaseSettings.Value.FilmsCollectionName);
    }

    public async Task<GetFilmDto?> GetAsync(string id)
    {
        Film? film = await _filmCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        return film is not null ? new GetFilmDto(film) : null;
    }
    public async Task<List<GetFilmDto>> GetAllAsync()
    {
        List<Film> films = await _filmCollection.Find(_ => true).ToListAsync();
        return films.Select(x => new GetFilmDto(x)).ToList();
    }

    public async Task CreateAsync(CreateFilmDto createFilmDto) => await _filmCollection.InsertOneAsync(new(createFilmDto));

    public async Task UpdateAsync(string id, CreateFilmDto updatedFilm) => await _filmCollection.ReplaceOneAsync(x => x.Id == id, new(updatedFilm));

    public async Task RemoveAsync(string id) => await _filmCollection.DeleteOneAsync(x => x.Id == id);
}
