using FilmForumWebAPI.Models.Dtos.ActorDtos;
using FilmForumWebAPI.Models.Entities.BaseEntities;

namespace FilmForumWebAPI.Models.Entities;

public class Actor : BaseMongoDatabaseEntity
{
    public Actor()
    {
    }

    public Actor(CreateActorDto createActorDto)
    {
        Name = createActorDto.Name;
        Surname = createActorDto.Surname;
        Description = createActorDto.Description;
    }

    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}