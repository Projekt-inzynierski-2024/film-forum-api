﻿namespace FilmForumWebAPI.Models.Entities;

public class Film : BaseDbEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
