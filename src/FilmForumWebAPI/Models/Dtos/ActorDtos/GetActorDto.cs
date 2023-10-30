using FilmForumWebAPI.Models.Entities;
using MongoDB.Bson.Serialization.Attributes;

namespace FilmForumWebAPI.Models.Dtos.ActorDtos;

public class GetActorDto
{
    public GetActorDto(Actor actor)
    {
        Id = actor.Id;
        Name = actor.Name;
        Surname = actor.Surname;
        Description = actor.Description;
    }

    public string Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Surname { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}
