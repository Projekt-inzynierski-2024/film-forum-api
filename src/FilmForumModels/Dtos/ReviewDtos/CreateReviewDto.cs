using System.ComponentModel.DataAnnotations;

namespace FilmForumModels.Dtos.ReviewDtos;

public class CreateReviewDto
{
    [Required]
    public string EpisodeId { get; set; } = string.Empty;

    [Required]
    [Range(0.0f, 10.0f, ErrorMessage = "Rate must be between 0.0 and 10.0")]
    public float Rate { get; set; } = 0.0f;

    [Required]
    [MaxLength(500, ErrorMessage = "Comment can't have more than 500 characters")]
    public string Comment { get; set; } = string.Empty;
}