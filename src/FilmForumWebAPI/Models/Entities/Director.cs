using FilmForumWebAPI.Models.Dtos.DirectorDtos;
using FilmForumWebAPI.Models.Entities.BaseEntities;
using MongoDB.Bson.Serialization.Attributes;

namespace FilmForumWebAPI.Models.Entities;

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

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("surname")]
    public string Surname { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;
}