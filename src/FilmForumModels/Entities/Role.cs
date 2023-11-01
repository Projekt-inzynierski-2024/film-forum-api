using FilmForumModels.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmForumModels.Entities;

[Table("role")]
public class Role : BaseMsSqlDatabaseEntity
{
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}