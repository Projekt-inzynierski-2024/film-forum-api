using FilmForumWebAPI.Database;
using FilmForumWebAPI.Models.Dtos.FilmDtos;
using FilmForumWebAPI.Models.Entities;
using FilmForumWebAPI.Services.Interfaces;
using MongoDB.Driver;

namespace FilmForumWebAPI.Services;

public class FilmService : IFilmService
{
    private readonly IMongoCollection<Film> _filmCollection;

    public FilmService(FilmsDatabaseContext filmsDatabaseContext)
    {
        _filmCollection = filmsDatabaseContext.FilmCollection;
    }

    public async Task<GetFilmDto?> GetAsync(string id)
        => await _filmCollection.Find(x => x.Id == id).FirstOrDefaultAsync() is Film film ? new(film) : null;

    public async Task<List<GetFilmDto>> GetAllAsync()
        => await _filmCollection.Find(_ => true).ToListAsync() is IEnumerable<Film> films ? films.Select(x => new GetFilmDto(x)).ToList() : new();

    public async Task CreateAsync(CreateFilmDto createFilmDto)
        => await _filmCollection.InsertOneAsync(new(createFilmDto));

    public async Task UpdateAsync(string id, CreateFilmDto updatedFilm)
        => await _filmCollection.ReplaceOneAsync(x => x.Id == id, new(updatedFilm));

    public async Task RemoveAsync(string id)
        => await _filmCollection.DeleteOneAsync(x => x.Id == id);
}