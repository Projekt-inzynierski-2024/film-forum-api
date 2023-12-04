namespace FilmForumModels.Models.Settings;

public class EmailSenderDetails
{
    public static string SectionKey { get; } = "EmailSenderDetails";
    public string SenderName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}