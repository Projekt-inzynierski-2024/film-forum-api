﻿namespace FilmForumModels.Models.Settings;

public class JwtDetails
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int LifetimeInMinutes { get; set; }
}