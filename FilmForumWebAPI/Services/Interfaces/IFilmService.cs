using FilmForumWebAPI.Models.Dtos.Film;
using FilmForumWebAPI.Models.Entities;

namespace FilmForumWebAPI.Services.Interfaces;

public interface IFilmService
{
    Task<List<Film>> GetAllAsync();

    Task<Film?> GetAsync(string id);

    Task CreateAsync(CreateFilmDto createFilmDto);

    Task UpdateAsync(string id, CreateFilmDto updatedFilm);

    Task RemoveAsync(string id);
}