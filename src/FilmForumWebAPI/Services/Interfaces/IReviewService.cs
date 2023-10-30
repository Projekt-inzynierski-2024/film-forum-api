using FilmForumWebAPI.Models.Dtos.ReviewDtos;

namespace FilmForumWebAPI.Services.Interfaces;

public interface IReviewService
{
    Task CreateAsync(CreateReviewDto createReviewDto);

    Task<List<GetReviewDto>> GetAllAsync();

    Task<GetReviewDto?> GetAsync(string id);

    Task UpdateAsync(string id, CreateReviewDto createReviewDto);

    Task RemoveAsync(string id);
}