using FilmForumModels.Models.Email;
using FilmForumModels.Models.Settings;

namespace EmailSender.Interfaces;

public interface IEmailService
{
    Task<string> SendEmailAsync(IEmailMessage emailMessage, EmailSenderDetails emailSenderDetails, SmtpSettings smtpSettings);
}