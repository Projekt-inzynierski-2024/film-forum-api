using FilmForumModels.Dtos.FilmDtos;

namespace FilmForumWebAPI.Services.Interfaces;

public interface IFilmService
{
    Task<List<GetFilmDto>> GetAllAsync();

    Task<List<GetDetailedFilmDto>> GetDetailedAllAsync();

    Task<GetFilmDto?> GetAsync(string id);

    Task<GetDetailedFilmDto?> GetDetailedAsync(string id);

    Task CreateAsync(CreateFilmDto createFilmDto);

    Task UpdateAsync(string id, CreateFilmDto updatedFilm);

    Task RemoveAsync(string id);
}