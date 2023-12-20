using FilmForumModels.Dtos.FilmDtos;
using FilmForumModels.Dtos.ReviewDtos;
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
    /// Returns list of films with titles matching <paramref name="query"/> from database
    /// </summary>
    /// <param name="query">Film's title</param>
    /// <returns>List of films with matching titles</returns>
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
    /// Returns list of all films from database
    /// </summary>
    /// <returns>List of all films</returns>
    public async Task<List<GetFilmDto>> GetAllAsync()
        => await _filmCollection.Find(_ => true).ToListAsync() is IEnumerable<Film> films ? films.Select(x => new GetFilmDto(x)).ToList() : new();

    /// <summary>
    /// Returns list of all films with details from database
    /// </summary>
    /// <returns>List of all films with details from database</returns>
    public async Task<List<GetDetailedFilmDto>> GetDetailedAllAsync()
       => await _detailedFilmCursor.ToListAsync() is IEnumerable<Film> films ? films.Select(x => new GetDetailedFilmDto(x)).ToList() : new();

    /// <summary>
    /// Adds new film to database
    /// </summary>
    /// <param name="createFilmDto">Details to create new film</param>
    /// <returns>The result of the insert operation</returns>
    public async Task CreateAsync(CreateFilmDto createFilmDto)
        => await _filmCollection.InsertOneAsync(new(createFilmDto));

    /// <summary>
    /// Updates film with given <paramref name="id"/> in database
    /// </summary>
    /// <param name="id">Film's id</param>
    /// <param name="updatedFilm">Details to update film</param>
    /// <returns>The result of the replacement</returns>
    public async Task<ReplaceOneResult> UpdateAsync(string id, CreateFilmDto updatedFilm)
        => await _filmCollection.ReplaceOneAsync(x => x.Id == id, new(id, updatedFilm));

    /// <summary>
    /// Deletes film with given <paramref name="id"/> from database
    /// </summary>
    /// <param name="id">Film's id</param>
    /// <returns>The result of the delete operation</returns>
    public async Task<DeleteResult> RemoveAsync(string id)
        => await _filmCollection.DeleteOneAsync(x => x.Id == id);
}