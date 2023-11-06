using FilmForumModels.Dtos.DirectorDtos;

namespace FilmForumWebAPI.Services.Interfaces;

public interface IDirectorService
{
    Task<List<GetDirectorDto>> SearchAllAsync(string query);

    Task CreateAsync(CreateDirectorDto createDirectorDto);

    Task<List<GetDirectorDto>> GetAllAsync();

    Task<GetDirectorDto?> GetAsync(string id);

    Task UpdateAsync(string id, CreateDirectorDto createDirectorDto);

    Task RemoveAsync(string id);
}
