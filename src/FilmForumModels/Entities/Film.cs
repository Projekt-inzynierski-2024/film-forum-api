using FilmForumModels.Dtos.FilmDtos;
using FilmForumModels.Entities.BaseEntities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FilmForumModels.Entities;

public class Film : BaseMongoDatabaseEntity
{
    public Film()
    {
    }

    public Film(CreateFilmDto createFilmDto)
    {
        Title = createFilmDto.Title;
        Description = createFilmDto.Description;
        IsMovie = createFilmDto.IsMovie;
    }

    public Film(string id, CreateFilmDto createFilmDto) : this(createFilmDto)
    {
        Id = id;
    }

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("isMovie")]
    public bool IsMovie { get; set; } = true;

    [BsonElement("episodes")]
    [BsonIgnoreIfNull]
    public List<Episode>? Episodes { get; private set; } = null;
}