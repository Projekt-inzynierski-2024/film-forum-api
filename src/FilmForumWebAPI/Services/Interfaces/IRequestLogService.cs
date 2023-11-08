using FilmForumModels.Dtos.RequestDtos;

namespace FilmForumWebAPI.Services.Interfaces;

public interface IRequestLogService
{
    Task<int> CreateAsync(CreateRequestLogDto createRequestLogDto);

    Task<List<GetRequestLogDto>> GetAllAsync();

    Task<GetRequestLogDto?> GetAsync(int id);

    Task RemoveAsync(int id);
}