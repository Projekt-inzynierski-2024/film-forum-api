using FilmForumModels.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmForumModels.Entities;

[Table("users")]
public class User : BaseMsSqlDatabaseEntity
{
    [Required, Column("username")]
    public string Username { get; set; } = string.Empty;

    [Required, Column("email")]
    public string Email { get; set; } = string.Empty;

    [Required, Column("password")]
    public string Password { get; set; } = string.Empty;

    [Column("2fa")]
    public bool MultifactorAuth { get; set; } = false;

    [Column("recover_password_token")]
    public string? RecoverPasswordToken { get; set; } = null;

    [Column("recover_password_token_expiration")]
    public DateTime? RecoverPasswordTokenExpiration { get; set; } = null;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
