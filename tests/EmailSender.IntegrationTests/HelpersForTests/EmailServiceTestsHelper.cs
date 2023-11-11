using FilmForumModels.Models.Settings;
using MailKit.Security;

namespace EmailSender.IntegrationTests.HelpersForTests;

public class EmailServiceTestsHelper
{
    public static EmailSenderDetails EmailSenderDetails => new()
    {
        SenderName = "FilmForumTests",
        Email = "filmforumwebtests@outlook.com",
        Password = "kd5#1D922d"
    };

    public static SmtpSettings SmtpSettings => new()
    {
        Host = "smtp-mail.outlook.com",
        Port = 587,
        SecureSocketOptions = SecureSocketOptions.StartTls,
    };
}