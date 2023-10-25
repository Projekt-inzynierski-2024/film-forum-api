﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmForumWebAPI.Models.Entities;

[Table("users")]
public class User : BaseDbEntity
{
    [Required, Column("username")]
    public string Username { get; set; } = string.Empty;

    [Required, Column("email")]
    public string Email { get; set; } = string.Empty;

    [Required, Column("password")]
    public string Password { get; set; } = string.Empty;
}