using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmForumModels.Entities.BaseEntities;

public abstract class BaseMsSqlDatabaseEntity
{
    [Key, Column("id")]
    public int Id { get; set; }
}