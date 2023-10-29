using FilmForumWebAPI.Models.Dtos.ReviewDtos;
using FilmForumWebAPI.Models.Entities.BaseEntities;

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

    public string UserId { get; set; } = string.Empty;

    public string EpisodeId { get; set; } = string.Empty;

    public float Rate { get; set; } = 0.0f;

    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}