using FilmForumModels.Dtos.EpisodeDtos;
using FilmForumModels.Entities.BaseEntities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FilmForumModels.Entities;

public class Episode : BaseMongoDatabaseEntity
{
    public Episode()
    {
    }

    public Episode(CreateEpisodeDto createEpisodeDto)
    {
        Title = createEpisodeDto.Title;
        Description = createEpisodeDto.Description;
        EpisodeNumber = createEpisodeDto.EpisodeNumber;
        SeasonNumber = createEpisodeDto.SeasonNumber;
        Length = createEpisodeDto.Length;
        Year = createEpisodeDto.Year;
        FilmId = createEpisodeDto.FilmId;
        DirectorIds = createEpisodeDto.DirectorIds;
        ActorIds = createEpisodeDto.ActorIds;
    }

    public Episode(string id, CreateEpisodeDto createEpisodeDto) : this(createEpisodeDto)
    {
        Id = id;
    }

    [BsonElement("title")]
    [BsonIgnoreIfNull]
    public string? Title { get; set; } = null;

    [BsonElement("description")]
    [BsonIgnoreIfNull]
    public string? Description { get; set; } = null;

    [BsonElement("episodeNumber")]
    [BsonIgnoreIfNull]
    public int? EpisodeNumber { get; set; } = null;

    [BsonElement("seasonNumber")]
    [BsonIgnoreIfNull]
    public int? SeasonNumber { get; set; } = null;

    [BsonElement("length")]
    public int Length { get; set; } = 0;

    [BsonElement("year")]
    public int Year { get; set; } = 2023;

    [BsonElement("filmId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string FilmId { get; set; } = string.Empty;

    [BsonElement("directorIds")]
    [BsonRepresentation(BsonType.ObjectId)]
    public List<string> DirectorIds { get; set; } = new List<string>();

    [BsonElement("directors")]
    [BsonIgnoreIfNull]
    public List<Director>? Directors { get; private set; } = null;

    [BsonElement("actorIds")]
    [BsonRepresentation(BsonType.ObjectId)]
    public List<string> ActorIds { get; set; } = new List<string>();

    [BsonElement("actors")]
    [BsonIgnoreIfNull]
    public List<Actor>? Actors { get; private set; } = null;

    [BsonElement("film")]
    [BsonIgnoreIfNull]
    public Film? Film { get; private set; } = null;

    [BsonElement("reviews")]
    [BsonIgnoreIfNull]
    public List<Review>? Reviews { get; private set; } = null;
}
