using FilmForumWebAPI.Database;
using FilmForumWebAPI.Models.Dtos.DirectorDtos;
using FilmForumWebAPI.Models.Entities;
using FilmForumWebAPI.Services.Interfaces;
using MongoDB.Driver;

namespace FilmForumWebAPI.Services;

public class DirectorService : IDirectorService
{
    private readonly IMongoCollection<Director> _directorCollection;

    public DirectorService(FilmsDatabaseContext filmsDatabaseContext)
    {
        _directorCollection = filmsDatabaseContext.DirectorCollection;
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