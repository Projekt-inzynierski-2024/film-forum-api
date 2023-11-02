using EmailSender.Interfaces;
using FilmForumModels.Models.Email;
using FilmForumModels.Models.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace EmailSender.Services;

public class EmailService : IEmailService
{
    public async Task SendEmailAsync(EmailMessage emailMessage, EmailSenderDetails emailSenderDetails)
    {
        MimeMessage emailToSend = new()
        {
            Sender = new MailboxAddress(emailSenderDetails.SenderName, emailSenderDetails.Email)
        };
        emailToSend.To.Add(MailboxAddress.Parse(emailMessage.To));
        emailToSend.Subject = emailMessage.Subject;
        emailToSend.Body = new TextPart(TextFormat.Html) { Text = emailMessage.Body };

        using SmtpClient smtp = new();
        await smtp.ConnectAsync("smtp-mail.outlook.com", 587, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(emailSenderDetails.Email, emailSenderDetails.Password);
        await smtp.SendAsync(emailToSend);
        await smtp.DisconnectAsync(true);
    }
}