using FilmForumModels.Dtos.ActorDtos;

namespace FilmForumWebAPI.Services.Interfaces;

public interface IActorService
{
    Task<List<GetActorDto>> SearchAllAsync(string query);

    Task CreateAsync(CreateActorDto createActorDto);

    Task<List<GetActorDto>> GetAllAsync();

    Task<GetActorDto?> GetAsync(string id);

    Task UpdateAsync(string id, CreateActorDto createActorDto);

    Task RemoveAsync(string id);
}
