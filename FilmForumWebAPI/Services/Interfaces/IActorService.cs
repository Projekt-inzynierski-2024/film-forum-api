using FilmForumWebAPI.Models.Dtos.Actor;

namespace FilmForumWebAPI.Services.Interfaces;

public interface IActorService
{
    Task<int> CreateAsync(CreateActorDto createActorDto);

    Task<GetActorDto?> GetAsync(string id);

    Task<List<GetActorDto>> GetAllAsync();
}