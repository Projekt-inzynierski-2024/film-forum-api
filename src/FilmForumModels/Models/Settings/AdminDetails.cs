namespace FilmForumModels.Models.Settings;

public class AdminDetails
{
    public static string SectionKey { get; } = "AdminDetails";
    public string SecretKey { get; set; } = string.Empty;
}