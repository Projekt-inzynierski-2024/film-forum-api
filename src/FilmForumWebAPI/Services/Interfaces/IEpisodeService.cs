using FilmForumWebAPI.Models.Dtos.EpisodeDtos;

namespace FilmForumWebAPI.Services.Interfaces;

public interface IEpisodeService
{
    Task CreateAsync(CreateEpisodeDto createEpisodeDto);

    Task<List<GetEpisodeDto>> GetAllAsync();

    Task<GetEpisodeDto?> GetAsync(string id);

    Task UpdateAsync(string id, CreateEpisodeDto createEpisodeDto);

    Task RemoveAsync(string id);
}