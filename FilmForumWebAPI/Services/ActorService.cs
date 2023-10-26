using FilmForumWebAPI.Models.Dtos.Actor;
using FilmForumWebAPI.Services.Interfaces;

namespace FilmForumWebAPI.Services;

public class ActorService : IActorService
{
    public async Task<GetActorDto> GetActorAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<int> CreateActorAsync(CreateActorDto createActorDto)
    {
        throw new NotImplementedException();
    }
}
