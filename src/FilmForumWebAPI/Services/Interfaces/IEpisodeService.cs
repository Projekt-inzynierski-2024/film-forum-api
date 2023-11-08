using FilmForumModels.Dtos.EpisodeDtos;

namespace FilmForumWebAPI.Services.Interfaces;

public interface IEpisodeService
{
    Task<List<GetEpisodeDto>> SearchAllAsync(string query);

    Task CreateAsync(CreateEpisodeDto createEpisodeDto);

    Task<List<GetEpisodeDto>> GetAllAsync();

    Task<List<GetDetailedEpisodeDto>> GetDetailedAllAsync();

    Task<GetEpisodeDto?> GetAsync(string id);

    Task<GetDetailedEpisodeDto?> GetDetailedAsync(string id);

    Task UpdateAsync(string id, CreateEpisodeDto createEpisodeDto);

    Task RemoveAsync(string id);
}