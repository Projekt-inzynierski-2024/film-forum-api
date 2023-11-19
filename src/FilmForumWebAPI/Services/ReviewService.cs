using FilmForumModels.Dtos.ReviewDtos;
using FilmForumModels.Entities;
using FilmForumWebAPI.Database;
using FilmForumWebAPI.Services.Interfaces;
using MongoDB.Driver;

namespace FilmForumWebAPI.Services;

public class ReviewService : IReviewService
{
    private readonly IMongoCollection<Review> _reviewCollection;

    public ReviewService(FilmsDatabaseContext filmsDatabaseContext) => _reviewCollection = filmsDatabaseContext.ReviewCollection;

    public async Task CreateAsync(string userId, CreateReviewDto createReviewDto) => await _reviewCollection.InsertOneAsync(new(userId, createReviewDto));

    public async Task<List<GetReviewDto>> GetAllAsync()
        => await _reviewCollection.Find(_ => true).ToListAsync() is IEnumerable<Review> reviews ? reviews.Select(x => new GetReviewDto(x)).ToList() : new();

    public async Task<GetReviewDto?> GetAsync(string id)
        => await _reviewCollection.Find(x => x.Id == id).FirstOrDefaultAsync() is Review review ? new(review) : null;

    public async Task<ReplaceOneResult> UpdateAsync(string id, string userId, CreateReviewDto createReviewDto)
        => await _reviewCollection.ReplaceOneAsync(x => x.Id == id, new(id, userId, createReviewDto));

    public async Task<DeleteResult> RemoveAsync(string id)
        => await _reviewCollection.DeleteOneAsync(x => x.Id == id);
}