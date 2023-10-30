using System.ComponentModel.DataAnnotations;

namespace FilmForumModels.Dtos.DirectorDtos;

public class CreateDirectorDto
{
    [Required]
    [MaxLength(64, ErrorMessage = "Name can't have more than 64 characters")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(64, ErrorMessage = "Surname can't have more than 64 characters")]
    public string Surname { get; set; } = string.Empty;

    [Required]
    [MaxLength(500, ErrorMessage = "Description can't have more than 500 characters")]
    public string Description { get; set; } = string.Empty;
}