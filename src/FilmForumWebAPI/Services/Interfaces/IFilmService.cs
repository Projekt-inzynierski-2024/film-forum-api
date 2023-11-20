using FilmForumModels.Dtos.FilmDtos;
using MongoDB.Driver;

namespace FilmForumWebAPI.Services.Interfaces;

public interface IFilmService
{
    Task<List<GetFilmDto>> SearchAllAsync(string query);

    Task<List<GetFilmDto>> GetAllAsync();

    Task<List<GetDetailedFilmDto>> GetDetailedAllAsync();

    Task<GetFilmDto?> GetAsync(string id);

    Task<GetDetailedFilmDto?> GetDetailedAsync(string id);

    Task CreateAsync(CreateFilmDto createFilmDto);

    Task<ReplaceOneResult> UpdateAsync(string id, CreateFilmDto updatedFilm);

    Task<DeleteResult> RemoveAsync(string id);
}