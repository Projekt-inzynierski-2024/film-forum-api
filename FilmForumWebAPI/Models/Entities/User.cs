using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FilmForumWebAPI.Models.Entities.BaseEntities;

namespace FilmForumWebAPI.Models.Entities;

[Table("users")]
public class User : BaseMsSqlDatabaseEntity
{
    [Required, Column("username")]
    public string Username { get; set; } = string.Empty;

    [Required, Column("email")]
    public string Email { get; set; } = string.Empty;

    [Required, Column("password")]
    public string Password { get; set; } = string.Empty;

    [Column("recover_password_token")]
    public string? RecoverPasswordToken { get; set; } = null;

    [Column("recover_password_token_expiration")]
    public DateTime? RecoverPasswordTokenExpiration { get; set; } = null;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}