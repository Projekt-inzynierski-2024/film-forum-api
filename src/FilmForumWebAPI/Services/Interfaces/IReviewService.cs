using FilmForumModels.Dtos.ReviewDtos;
using MongoDB.Driver;

namespace FilmForumWebAPI.Services.Interfaces;

public interface IReviewService
{
    Task CreateAsync(CreateReviewDto createReviewDto);

    Task<List<GetReviewDto>> GetAllAsync();

    Task<GetReviewDto?> GetAsync(string id);

    Task<ReplaceOneResult> UpdateAsync(string id, CreateReviewDto createReviewDto);

    Task RemoveAsync(string id);
}