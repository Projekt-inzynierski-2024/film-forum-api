using FilmForumModels.Dtos.DirectorDtos;
using FilmForumModels.Entities.BaseEntities;
using MongoDB.Bson.Serialization.Attributes;

namespace FilmForumModels.Entities;

public class Director : BaseMongoDatabaseEntity
{
    public Director()
    {
    }

    public Director(CreateDirectorDto createDirectorDto)
    {
        Name = createDirectorDto.Name;
        Surname = createDirectorDto.Surname;
        Description = createDirectorDto.Description;
    }

    public Director(String id, CreateDirectorDto createDirectorDto) : this(createDirectorDto)
    {
        Id = id;
    }

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("surname")]
    public string Surname { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;
}