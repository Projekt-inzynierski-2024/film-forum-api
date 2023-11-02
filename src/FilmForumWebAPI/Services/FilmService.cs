using FilmForumModels.Dtos.FilmDtos;
using FilmForumModels.Entities;
using FilmForumWebAPI.Database;
using FilmForumWebAPI.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FilmForumWebAPI.Services;

public class FilmService : IFilmService
{
    private readonly IMongoCollection<Film> _filmCollection;
    private readonly IAsyncCursor<Film> _detailedFilmCursor;

    public FilmService(FilmsDatabaseContext filmsDatabaseContext)
    {
        _filmCollection = filmsDatabaseContext.FilmCollection;

        BsonDocument[] pipeline = new BsonDocument[]
        {
            new BsonDocument
            (
                "$lookup", new BsonDocument
                {
                    { "from",         "episode"  },
                    { "foreignField", "filmId"   },
                    { "localField",   "_id"      },
                    { "as",           "episodes" },
                }
            )
        };

        _detailedFilmCursor = _filmCollection.Aggregate<Film>(pipeline);
    }

    public async Task<GetFilmDto?> GetAsync(string id)
        => await _filmCollection.Find(x => x.Id == id).FirstOrDefaultAsync() is Film film ? new(film) : null;

    public async Task<GetDetailedFilmDto?> GetDetailedAsync(string id)
        => await _detailedFilmCursor.ToListAsync().ContinueWith(filmsTask => filmsTask.Result.Find(x => x.Id == id)) is Film film ? new(film) : null;

    public async Task<List<GetFilmDto>> GetAllAsync()
        => await _filmCollection.Find(_ => true).ToListAsync() is IEnumerable<Film> films ? films.Select(x => new GetFilmDto(x)).ToList() : new();

    public async Task<List<GetDetailedFilmDto>> GetDetailedAllAsync()
       => await _detailedFilmCursor.ToListAsync() is IEnumerable<Film> films ? films.Select(x => new GetDetailedFilmDto(x)).ToList() : new();

    public async Task CreateAsync(CreateFilmDto createFilmDto)
        => await _filmCollection.InsertOneAsync(new(createFilmDto));

    public async Task UpdateAsync(string id, CreateFilmDto updatedFilm)
        => await _filmCollection.ReplaceOneAsync(x => x.Id == id, new(updatedFilm));

    public async Task RemoveAsync(string id)
        => await _filmCollection.DeleteOneAsync(x => x.Id == id);
}
