namespace FilmForumWebAPI.Models.Dtos.Actor;

public class GetActorDto
{
    public GetActorDto(Entities.Actor actor)
    {
        Id = actor.Id;
    }

    public string Id { get; set; }
}
