﻿using FilmForumWebAPI.Models;
using FilmForumWebAPI.Models.Dtos.EpisodeDtos;
using FilmForumWebAPI.Models.Entities;
using FilmForumWebAPI.Services.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FilmForumWebAPI.Services;

public class EpisodeService : IEpisodeService
{
    private readonly IMongoCollection<Episode> _episodeCollection;

    public EpisodeService(IOptions<FilmForumMongoDatabaseSettings> mongoDatabaseSettings)
    {
        MongoClient mongoClient = new(mongoDatabaseSettings.Value.ConnectionString);

        IMongoDatabase mongoDatabase = mongoClient.GetDatabase(mongoDatabaseSettings.Value.DatabaseName);

        _episodeCollection = mongoDatabase.GetCollection<Episode>(mongoDatabaseSettings.Value.EpisodesCollectionName);
    }

    public async Task CreateAsync(CreateEpisodeDto createEpisodeDto) => await _episodeCollection.InsertOneAsync(new(createEpisodeDto));

    public async Task<List<GetEpisodeDto>> GetAllAsync()
        => await _episodeCollection.Find(_ => true).ToListAsync() is IEnumerable<Episode> episodes ? episodes.Select(x => new GetEpisodeDto(x)).ToList() : new();

    public async Task<GetEpisodeDto?> GetAsync(string id)
        => await _episodeCollection.Find(x => x.Id == id).FirstOrDefaultAsync() is Episode episode ? new(episode) : null;

    public async Task UpdateAsync(string id, CreateEpisodeDto createEpisodeDto)
        => await _episodeCollection.ReplaceOneAsync(x => x.Id == id, new(createEpisodeDto));

    public async Task RemoveAsync(string id)
        => await _episodeCollection.DeleteOneAsync(x => x.Id == id);
}