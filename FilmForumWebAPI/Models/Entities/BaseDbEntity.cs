using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmForumWebAPI.Models.Entities;

public abstract class BaseDbEntity
{
    [Key, Column("id")]
    public int Id { get; set; }
}
