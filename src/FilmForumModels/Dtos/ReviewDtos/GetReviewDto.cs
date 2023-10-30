using FilmForumModels.Entities;

namespace FilmForumModels.Dtos.ReviewDtos;

public class GetReviewDto
{
    public GetReviewDto(Review review)
    {
        Id = review.Id;
        UserId = review.UserId;
        EpisodeId = review.EpisodeId;
        Rate = review.Rate;
        Comment = review.Comment;
        CreatedAt = review.CreatedAt;
    }

    public string Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string EpisodeId { get; set; } = string.Empty;

    public float Rate { get; set; }

    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}