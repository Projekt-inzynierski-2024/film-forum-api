using FilmForumModels.Dtos.RequestDtos;

namespace FilmForumWebAPI.Services.Interfaces;

public interface IRequestLogService
{
    Task<int> CreateAsync(CreateRequestLogDto createRequestLogDto);

    Task<List<GetRequestLogDto>> GetAllAsync();

    Task<GetRequestLogDto?> GetAsync(int id);

    Task<List<GetRequestLogDto>> GetUserAllRequestsLogsAsync(int userId);

    Task<int> RemoveAsync(int id);

    Task<int> RemoveUserRequestsLogsAsync(int userId);
}