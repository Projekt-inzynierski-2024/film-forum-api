﻿namespace FilmForumModels.Models.Email;

public class EmailMessage : IEmailMessage
{
    public required string To { get; set; }
    public string? Subject { get; set; }
    public string? Body { get; set; }
}