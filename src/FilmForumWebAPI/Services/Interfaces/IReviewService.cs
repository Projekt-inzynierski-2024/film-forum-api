using FilmForumModels.Dtos.ReviewDtos;
using MongoDB.Driver;

namespace FilmForumWebAPI.Services.Interfaces;

public interface IReviewService
{
    Task CreateAsync(string userId, CreateReviewDto createReviewDto);

    Task<List<GetReviewDto>> GetAllAsync();

    Task<GetReviewDto?> GetAsync(string id);

    Task<ReplaceOneResult> UpdateAsync(string id, string userId, CreateReviewDto createReviewDto);

    Task RemoveAsync(string id);
}