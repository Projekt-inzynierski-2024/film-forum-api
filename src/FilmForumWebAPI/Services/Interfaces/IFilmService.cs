using FilmForumModels.Dtos.FilmDtos;

namespace FilmForumWebAPI.Services.Interfaces;

public interface IFilmService
{
    Task<List<GetFilmDto>> GetAllAsync();

    Task<GetFilmDto?> GetAsync(string id);

    Task CreateAsync(CreateFilmDto createFilmDto);

    Task UpdateAsync(string id, CreateFilmDto updatedFilm);

    Task RemoveAsync(string id);
}