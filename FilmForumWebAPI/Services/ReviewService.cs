using FilmForumWebAPI.Models.Dtos.EpisodeDtos;
using FilmForumWebAPI.Models.Entities;
using FilmForumWebAPI.Models;
using FilmForumWebAPI.Services.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using FilmForumWebAPI.Models.Dtos.ReviewDtos;

namespace FilmForumWebAPI.Services;

public class ReviewService : IReviewService
{
    private readonly IMongoCollection<Review> _reviewCollection;

    public ReviewService(IOptions<FilmForumMongoDatabaseSettings> mongoDatabaseSettings)
    {
        MongoClient mongoClient = new(mongoDatabaseSettings.Value.ConnectionString);

        IMongoDatabase mongoDatabase = mongoClient.GetDatabase(mongoDatabaseSettings.Value.DatabaseName);

        _reviewCollection = mongoDatabase.GetCollection<Review>(mongoDatabaseSettings.Value.EpisodesCollectionName);
    }

    public async Task CreateAsync(CreateReviewDto createReviewDto) => await _reviewCollection.InsertOneAsync(new(createReviewDto));

    public async Task<List<GetReviewDto>> GetAllAsync()
        => await _reviewCollection.Find(_ => true).ToListAsync() is IEnumerable<Review> reviews ? reviews.Select(x => new GetReviewDto(x)).ToList() : new();

    public async Task<GetReviewDto?> GetAsync(string id)
        => await _reviewCollection.Find(x => x.Id == id).FirstOrDefaultAsync() is Review review ? new(review) : null;

    public async Task UpdateAsync(string id, CreateReviewDto createReviewDto)
        => await _reviewCollection.ReplaceOneAsync(x => x.Id == id, new(createReviewDto));

    public async Task RemoveAsync(string id)
        => await _reviewCollection.DeleteOneAsync(x => x.Id == id);
}