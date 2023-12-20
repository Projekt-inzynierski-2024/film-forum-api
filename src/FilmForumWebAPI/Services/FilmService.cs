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
            new
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

    /// <summary>
    /// Returns list of film with titles matching <paramref name="query"/> from database
    /// </summary>
    /// <param name="query">Film's title</param>
    /// <returns>List of film with matching titles</returns>
    public async Task<List<GetFilmDto>> SearchAllAsync(string query)
        => await _filmCollection.Find(x => x.Title.ToUpper().Contains(query.ToUpper())).ToListAsync() is IEnumerable<Film> films ? films.Select(x => new GetFilmDto(x)).ToList() : new();

    /// <summary>
    /// Returns film with given <paramref name="id"/> from database
    /// </summary>
    /// <param name="id">Film's id</param>
    /// <returns>Film if found otherwise null</returns>
    public async Task<GetFilmDto?> GetAsync(string id)
        => await _filmCollection.Find(x => x.Id == id).FirstOrDefaultAsync() is Film film ? new(film) : null;

    /// <summary>
    /// Returns film with details with given <paramref name="id"/> from database
    /// </summary>
    /// <param name="id">Film's id</param>
    /// <returns>Film with details if found otherwise null</returns>
    public async Task<GetDetailedFilmDto?> GetDetailedAsync(string id)
        => await _detailedFilmCursor.ToListAsync().ContinueWith(filmsTask => filmsTask.Result.Find(x => x.Id == id)) is Film film ? new(film) : null;

    /// <summary>
    /// Returns list of all film from database
    /// </summary>
    /// <returns>List of all film</returns>
    public async Task<List<GetFilmDto>> GetAllAsync()
        => await _filmCollection.Find(_ => true).ToListAsync() is IEnumerable<Film> films ? films.Select(x => new GetFilmDto(x)).ToList() : new();

    public async Task<List<GetDetailedFilmDto>> GetDetailedAllAsync()
       => await _detailedFilmCursor.ToListAsync() is IEnumerable<Film> films ? films.Select(x => new GetDetailedFilmDto(x)).ToList() : new();

    public async Task CreateAsync(CreateFilmDto createFilmDto)
        => await _filmCollection.InsertOneAsync(new(createFilmDto));

    public async Task<ReplaceOneResult> UpdateAsync(string id, CreateFilmDto updatedFilm)
        => await _filmCollection.ReplaceOneAsync(x => x.Id == id, new(id, updatedFilm));

    public async Task<DeleteResult> RemoveAsync(string id)
        => await _filmCollection.DeleteOneAsync(x => x.Id == id);
}