﻿using FilmForumModels.Dtos.EpisodeDtos;
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
            new
            (
                "$lookup", new BsonDocument
                {
                    { "from",         "film"  },
                    { "foreignField", "_id"   },
                    { "localField",   "filmId"      },
                    { "as",           "film" },
                }
            ),
            new("$unwind", "$film"),
            new
            (
                "$lookup", new BsonDocument
                {
                    { "from",         "director" },
                    { "foreignField", "_id" },
                    { "localField",   "directorIds" },
                    { "as",           "directors" },
                }
            ),
            new
            (
                "$lookup", new BsonDocument
                {
                    { "from",         "actor" },
                    { "foreignField", "_id" },
                    { "localField",   "actorIds" },
                    { "as",           "actors" },
                }
            ),
            new
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

    /// <summary>
    /// Returns list of episodes with titles matching <paramref name="query"/> from database
    /// </summary>
    /// <param name="query">Episode's title</param>
    /// <returns>List of episodes with matching titles</returns>
    public async Task<List<GetEpisodeDto>> SearchAllAsync(string query)
        => await _episodeCollection.Find(x => (x.Title ?? "").ToUpper().Contains(query.ToUpper())).ToListAsync() is IEnumerable<Episode> episodes ? episodes.Select(x => new GetEpisodeDto(x)).ToList() : new();

    /// <summary>
    /// Adds new episode to database
    /// </summary>
    /// <param name="createEpisodeDto">Details to create new episode</param>
    /// <returns>The result of the insert operation</returns>
    public async Task CreateAsync(CreateEpisodeDto createEpisodeDto)
        => await _episodeCollection.InsertOneAsync(new(createEpisodeDto));

    /// <summary>
    /// Returns list of all episodes from database
    /// </summary>
    /// <returns>List of all episodes</returns>
    public async Task<List<GetEpisodeDto>> GetAllAsync()
        => await _episodeCollection.Find(_ => true).ToListAsync() is IEnumerable<Episode> episodes ? episodes.Select(x => new GetEpisodeDto(x)).ToList() : new();

    /// <summary>
    /// Returns list of all episodes with details from database
    /// </summary>
    /// <returns>List of all episodes with details</returns>
    public async Task<List<GetDetailedEpisodeDto>> GetDetailedAllAsync()
        => await _detailedEpisodeCursor.ToListAsync() is IEnumerable<Episode> episodes ? episodes.Select(x => new GetDetailedEpisodeDto(x)).ToList() : new();

    /// <summary>
    /// Returns episode with given <paramref name="id"/> from database
    /// </summary>
    /// <param name="id">Episode's id</param>
    /// <returns>Episode if found otherwise null</returns>
    public async Task<GetEpisodeDto?> GetAsync(string id)
        => await _episodeCollection.Find(x => x.Id == id).FirstOrDefaultAsync() is Episode episode ? new(episode) : null;

    /// <summary>
    /// Returns episode with details with given <paramref name="id"/> from database
    /// </summary>
    /// <param name="id">Episode's id</param>
    /// <returns>Episode with details if found otherwise null</returns>
    public async Task<GetDetailedEpisodeDto?> GetDetailedAsync(string id)
        => await _detailedEpisodeCursor.ToListAsync().ContinueWith(episodeTask => episodeTask.Result.Find(x => x.Id == id)) is Episode episode ? new(episode) : null;

    /// <summary>
    /// Updates episode with given <paramref name="id"/> in database
    /// </summary>
    /// <param name="id">Episode's id</param>
    /// <param name="createEpisodeDto">Details to update episode</param>
    /// <returns>The result of the replacement</returns>
    public async Task<ReplaceOneResult> UpdateAsync(string id, CreateEpisodeDto createEpisodeDto)
        => await _episodeCollection.ReplaceOneAsync(x => x.Id == id, new(id, createEpisodeDto));

    /// <summary>
    /// Deletes episode with given <paramref name="id"/> from database
    /// </summary>
    /// <param name="id">Episode's id</param>
    /// <returns>The result of the delete operation</returns>
    public async Task<DeleteResult> RemoveAsync(string id)
        => await _episodeCollection.DeleteOneAsync(x => x.Id == id);
}