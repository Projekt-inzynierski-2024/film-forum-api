using FilmForumWebAPI.Models.Dtos.ReviewDtos;
using FilmForumWebAPI.Models.Entities.BaseEntities;
using MongoDB.Bson.Serialization.Attributes;

namespace FilmForumWebAPI.Models.Entities;

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

    [BsonElement("userId")]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("episodeId")]
    public string EpisodeId { get; set; } = string.Empty;

    [BsonElement("rate")]
    public float Rate { get; set; } = 0.0f;

    [BsonElement("comment")]
    public string Comment { get; set; } = string.Empty;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}