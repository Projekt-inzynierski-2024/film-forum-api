using System.ComponentModel.DataAnnotations;

namespace FilmForumWebAPI.Models.Dtos.FilmDtos;

public class CreateFilmDto
{
    [Required]
    [MaxLength(100, ErrorMessage = "Title can't have more than 100 characters")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(500, ErrorMessage = "Description can't have more than 500 characters")]
    public string Description { get; set; } = string.Empty;
}