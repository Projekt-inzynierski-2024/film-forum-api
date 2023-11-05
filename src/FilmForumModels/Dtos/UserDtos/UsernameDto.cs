using System.ComponentModel.DataAnnotations;

namespace FilmForumModels.Dtos.UserDtos;

public class UsernameDto
{
    [Required(ErrorMessage = "Username is required")]
    [MinLength(5, ErrorMessage = "Username must have at least 5 characters")]
    [MaxLength(50, ErrorMessage = "Username can't have more than 50 characters")]
    public string Username { get; set; } = string.Empty;
}