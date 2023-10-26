using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using FilmForumWebAPI.Models.Dtos.Film;

namespace FilmForumWebAPI.Models.Entities;

public class Film
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    public static Film of(CreateFilmDto dto)
    {
        return new Film {
            Title = dto.Title,
            Description = dto.Description
        };
    }
}
