using System.ComponentModel.DataAnnotations;

namespace FilmForumModels.Dtos.UserDtos;

public class EmailDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}