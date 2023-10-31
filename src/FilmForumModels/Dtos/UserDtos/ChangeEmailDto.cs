using System.ComponentModel.DataAnnotations;

namespace FilmForumModels.Dtos.UserDtos;

public class ChangeEmailDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}