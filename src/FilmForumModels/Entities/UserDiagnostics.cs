using FilmForumModels.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmForumModels.Entities;

/// <summary>
/// This class is added once to database when user is created
/// and then the class's properties are updated when user logs in, changes password or takes another tracked actions
/// </summary>
[Table("user_diagnostics")]
public class UserDiagnostics : BaseMsSqlDatabaseEntity
{
    public UserDiagnostics(int userId) => UserId = userId;

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("last_successfull_sign_in")]
    public DateTime LastSuccessfullSignIn { get; set; } = DateTime.UtcNow;

    [Column("last_failed_sign_in")]
    public DateTime? LastFailedSignIn { get; set; }

    [Column("last_username_change")]
    public DateTime? LastUsernameChange { get; set; }

    [Column("last_email_change")]
    public DateTime? LastEmailChange { get; set; }

    [Column("last_password_change")]
    public DateTime? LastPasswordChange { get; set; }
}