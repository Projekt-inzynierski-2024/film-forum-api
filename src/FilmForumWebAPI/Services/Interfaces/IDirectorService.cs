using FilmForumModels.Dtos.DirectorDtos;
using MongoDB.Driver;

namespace FilmForumWebAPI.Services.Interfaces;

public interface IDirectorService
{
    Task<List<GetDirectorDto>> SearchAllAsync(string query);

    Task CreateAsync(CreateDirectorDto createDirectorDto);

    Task<List<GetDirectorDto>> GetAllAsync();

    Task<GetDirectorDto?> GetAsync(string id);

    Task<ReplaceOneResult> UpdateAsync(string id, CreateDirectorDto createDirectorDto);

    Task RemoveAsync(string id);
}