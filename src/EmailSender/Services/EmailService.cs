using EmailSender.Interfaces;
using FilmForumModels.Models.Email;
using FilmForumModels.Models.Settings;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace EmailSender.Services;

public class EmailService : IEmailService
{
    public async Task<string> SendEmailAsync(IEmailMessage emailMessage, EmailSenderDetails emailSenderDetails, SmtpSettings smtpSettings)
    {
        MimeMessage emailToSend = new()
        {
            Sender = new MailboxAddress(emailSenderDetails.SenderName, emailSenderDetails.Email)
        };
        emailToSend.To.Add(MailboxAddress.Parse(emailMessage.To));
        emailToSend.Subject = emailMessage.Subject;
        emailToSend.Body = new TextPart(TextFormat.Html) { Text = emailMessage.Body };

        using SmtpClient smtp = new();
        await smtp.ConnectAsync(smtpSettings.Host, smtpSettings.Port, smtpSettings.SecureSocketOptions);
        await smtp.AuthenticateAsync(emailSenderDetails.Email, emailSenderDetails.Password);
        string result = await smtp.SendAsync(emailToSend);
        await smtp.DisconnectAsync(true);
        return result;
    }
}