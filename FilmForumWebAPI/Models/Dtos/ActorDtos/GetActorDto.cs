using FilmForumWebAPI.Models.Entities;

namespace FilmForumWebAPI.Models.Dtos.ActorDtos;

public class GetActorDto
{
    public GetActorDto(Actor actor)
    {
        Id = actor.Id;
    }

    public string Id { get; set; }
}
