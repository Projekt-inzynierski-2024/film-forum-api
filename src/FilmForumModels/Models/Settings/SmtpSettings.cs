using MailKit.Security;

namespace FilmForumModels.Models.Settings;

public class SmtpSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public SecureSocketOptions SecureSocketOptions { get; set; }
}