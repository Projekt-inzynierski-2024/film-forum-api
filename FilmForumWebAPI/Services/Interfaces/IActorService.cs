using FilmForumWebAPI.Models.Dtos.Actor;

namespace FilmForumWebAPI.Services.Interfaces;

public interface IActorService
{
    Task<GetActorDto> GetActorAsync(int id);
    Task<int> CreateActorAsync(CreateActorDto createActorDto);
}
