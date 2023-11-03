using FilmForumModels.Dtos.ReviewDtos;
using FilmForumModels.Entities.BaseEntities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FilmForumModels.Entities;

public class Review : BaseMongoDatabaseEntity
{
    public Review()
    {
    }

    public Review(CreateReviewDto createReviewDto)
    {
        UserId = createReviewDto.UserId;
        EpisodeId = createReviewDto.EpisodeId;
        Rate = createReviewDto.Rate;
        Comment = createReviewDto.Comment;
    }

    public Review(string id, CreateReviewDto createReviewDto) : this(createReviewDto)
    {
        Id = id;
    }

    [BsonElement("userId")]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("episodeId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string EpisodeId { get; set; } = string.Empty;

    [BsonElement("rate")]
    public float Rate { get; set; } = 0.0f;

    [BsonElement("comment")]
    public string Comment { get; set; } = string.Empty;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}