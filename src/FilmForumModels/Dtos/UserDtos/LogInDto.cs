using System.ComponentModel.DataAnnotations;

namespace FilmForumModels.Dtos.UserDtos;

public class LogInDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Password { get; set; } = string.Empty;

    public string? TotpCode { get; set; } = null;
}
