using FilmForumModels.Dtos.ActorDtos;
using FilmForumModels.Entities.BaseEntities;
using MongoDB.Bson.Serialization.Attributes;

namespace FilmForumModels.Entities;

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

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("surname")]
    public string Surname { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;
}