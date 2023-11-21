using FilmForumModels.Dtos.ActorDtos;
using MongoDB.Driver;

namespace FilmForumWebAPI.Services.Interfaces;

public interface IActorService
{
    Task<List<GetActorDto>> SearchAllAsync(string query);

    Task CreateAsync(CreateActorDto createActorDto);

    Task<List<GetActorDto>> GetAllAsync();

    Task<GetActorDto?> GetAsync(string id);

    Task<ReplaceOneResult> UpdateAsync(string id, CreateActorDto createActorDto);

    Task<DeleteResult> RemoveAsync(string id);
}