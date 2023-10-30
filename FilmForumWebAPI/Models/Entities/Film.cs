using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using FilmForumWebAPI.Models.Dtos.FilmDtos;
using FilmForumWebAPI.Models.Entities.BaseEntities;

namespace FilmForumWebAPI.Models.Entities;

public class Film : BaseMongoDatabaseEntity
{
    public Film()
    {
    }   

    public Film(CreateFilmDto createFilmDto)
    {
        Title = createFilmDto.Title;
        Description = createFilmDto.Description;
    }

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("isMovie")]
    public bool IsMovie { get; set; } = true;

    // TODO: ignore on save
    [BsonElement("episodes")]
    public List<Episode> Episodes = new List<Episode>();
}
