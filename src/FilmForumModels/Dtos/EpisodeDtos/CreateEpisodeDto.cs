using System.ComponentModel.DataAnnotations;

namespace FilmForumModels.Dtos.EpisodeDtos;

public class CreateEpisodeDto
{
    [Required]
    [MaxLength(100, ErrorMessage = "Title can't have more than 100 characters")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(500, ErrorMessage = "Description can't have more than 500 characters")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Episode number can't be bigger than 2147483647")]
    public int EpisodeNumber { get; set; } = 0;

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Season number can't be bigger than 2147483647")]
    public int SeasonNumber { get; set; } = 0;

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Length can't be bigger than 3.4 x 10^38")]
    public float Length { get; set; } = 0.0f;

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Year number can't be bigger than 2147483647")]
    public int Year { get; set; } = 2023;

    [Required]
    [MaxLength(10000, ErrorMessage = "Film id can't have more than 10000 characters")]
    public string FilmId { get; set; } = string.Empty;

    [Required]
    public List<string> DirectorIds { get; set; } = new List<string>();

    [Required]
    public List<string> ActorIds { get; set; } = new List<string>();
}
