using FilmForumModels.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmForumModels.Entities;

[Table("user_to_role")]
public class UserToRole : BaseMsSqlDatabaseEntity
{
    public UserToRole()
    { }

    public UserToRole(int roleId, int userId)
    {
        RoleId = roleId;
        UserId = userId;
    }

    [Column("role_id")]
    public int RoleId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }
}