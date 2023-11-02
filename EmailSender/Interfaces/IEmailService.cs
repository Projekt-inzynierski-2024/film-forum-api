using FilmForumModels.Models.Email;
using FilmForumModels.Models.Settings;

namespace EmailSender.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(EmailMessage emailMessage, EmailSenderDetails emailSenderDetails);
}