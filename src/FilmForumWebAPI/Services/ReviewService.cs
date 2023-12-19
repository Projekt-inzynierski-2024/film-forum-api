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

    /// <summary>
    /// Adds new review to database
    /// </summary>
    /// <param name="userId">User's id</param>
    /// <param name="createReviewDto">Details to create a review</param>
    /// <returns>The result of the insert operation</returns>
    public async Task CreateAsync(string userId, CreateReviewDto createReviewDto) => await _reviewCollection.InsertOneAsync(new(userId, createReviewDto));

    /// <summary>
    /// Gets all reviews from database
    /// </summary>
    /// <returns>List of all reviews from database</returns>
    public async Task<List<GetReviewDto>> GetAllAsync()
        => await _reviewCollection.Find(_ => true).ToListAsync() is IEnumerable<Review> reviews ? reviews.Select(x => new GetReviewDto(x)).ToList() : new();

    /// <summary>
    /// Gets review with given <paramref name="id"/> from database
    /// </summary>
    /// <param name="id">Review's id</param>
    /// <returns>Review with given <paramref name="id"/></returns>
    public async Task<GetReviewDto?> GetAsync(string id)
        => await _reviewCollection.Find(x => x.Id == id).FirstOrDefaultAsync() is Review review ? new(review) : null;

    /// <summary>
    /// Updates review with given <paramref name="id"/> from database
    /// </summary>
    /// <param name="id">Review's id</param>
    /// <param name="userId">User's id</param>
    /// <param name="createReviewDto">Details to update review</param>
    /// <returns>The result of the replacement</returns>
    public async Task<ReplaceOneResult> UpdateAsync(string id, string userId, CreateReviewDto createReviewDto)
        => await _reviewCollection.ReplaceOneAsync(x => x.Id == id, new(id, userId, createReviewDto));

    /// <summary>
    /// Deletes review with given <paramref name="id"/> from database
    /// </summary>
    /// <param name="id">Review's id</param>
    /// <returns>The result of the delete operation</returns>
    public async Task<DeleteResult> RemoveAsync(string id)
        => await _reviewCollection.DeleteOneAsync(x => x.Id == id);
}