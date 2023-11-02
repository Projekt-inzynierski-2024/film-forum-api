using FilmForumModels.Dtos.EpisodeDtos;
using FilmForumModels.Entities;
using FilmForumWebAPI.Database;
using FilmForumWebAPI.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FilmForumWebAPI.Services;

public class EpisodeService : IEpisodeService
{
    private readonly IMongoCollection<Episode> _episodeCollection;
    private readonly IAsyncCursor<Episode> _detailedEpisodeCursor;

    public EpisodeService(FilmsDatabaseContext filmsDatabaseContext)
    {
        _episodeCollection = filmsDatabaseContext.EpisodeCollection;

        BsonDocument[] pipeline = new BsonDocument[]
        {
            new BsonDocument
            (
                "$lookup", new BsonDocument
                {
                    { "from",         "film"  },
                    { "foreignField", "_id"   },
                    { "localField",   "filmId"      },
                    { "as",           "film" },
                }
            ),
            new BsonDocument("$unwind", "$film"),
            new BsonDocument
            (
                "$lookup", new BsonDocument
                {
                    { "from",         "director" },
                    { "foreignField", "_id" },
                    { "localField",   "directorIds" },
                    { "as",           "directors" },
                }
            ),
            new BsonDocument
            (
                "$lookup", new BsonDocument
                {
                    { "from",         "actor" },
                    { "foreignField", "_id" },
                    { "localField",   "actorIds" },
                    { "as",           "actors" },
                }
            ),
            new BsonDocument
            (
                "$lookup", new BsonDocument
                {
                    { "from",         "review" },
                    { "foreignField", "episodeId" },
                    { "localField",   "_id" },
                    { "as",           "reviews" },
                }
            )
        };

        _detailedEpisodeCursor = _episodeCollection.Aggregate<Episode>(pipeline);
    }

    public async Task CreateAsync(CreateEpisodeDto createEpisodeDto) => await _episodeCollection.InsertOneAsync(new(createEpisodeDto));

    public async Task<List<GetEpisodeDto>> GetAllAsync()
        => await _episodeCollection.Find(_ => true).ToListAsync() is IEnumerable<Episode> episodes ? episodes.Select(x => new GetEpisodeDto(x)).ToList() : new();

    public async Task<List<GetDetailedEpisodeDto>> GetDetailedAllAsync()
        => await _detailedEpisodeCursor.ToListAsync() is IEnumerable<Episode> episodes ? episodes.Select(x => new GetDetailedEpisodeDto(x)).ToList() : new();

    public async Task<GetEpisodeDto?> GetAsync(string id)
        => await _episodeCollection.Find(x => x.Id == id).FirstOrDefaultAsync() is Episode episode ? new(episode) : null;

    public async Task<GetDetailedEpisodeDto?> GetDetailedAsync(string id)
        => await _detailedEpisodeCursor.ToListAsync().ContinueWith(episodeTask => episodeTask.Result.Find(x => x.Id == id)) is Episode episode ? new(episode) : null;

    public async Task UpdateAsync(string id, CreateEpisodeDto createEpisodeDto)
        => await _episodeCollection.ReplaceOneAsync(x => x.Id == id, new(id, createEpisodeDto));

    public async Task RemoveAsync(string id)
        => await _episodeCollection.DeleteOneAsync(x => x.Id == id);
}
