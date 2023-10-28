using FilmForumWebAPI.Models.Entities.BaseEntities;
using MongoDB.Bson.Serialization.Attributes;

namespace FilmForumWebAPI.Models.Entities;

public class Episode: BaseMongoDatabaseEntity
{
    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("episodeNumber")]
    public int EpisodeNumber { get; set; } = 0;

    [BsonElement("seasonNumber")]
    public int SeasonNumber { get; set; } = 0;

    [BsonElement("length")]
    public float Length { get; set; } = 0.0f;

    [BsonElement("year")]
    public int Year { get; set; } = 2023;

    [BsonElement("filmId")]
    public string FilmId { get; set; } = string.Empty;
}
