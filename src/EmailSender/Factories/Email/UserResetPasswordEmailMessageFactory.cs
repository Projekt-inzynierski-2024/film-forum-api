using EmailSender.Factories.Email.Interfaces;
using FilmForumModels.Models.Email;

namespace EmailSender.Factories.Email;

public class UserResetPasswordEmailMessageFactory : IEmailMessageFactory
{
    public IEmailMessage Create(string to, string? subject = null, string? body = null) => new EmailMessage
    {
        To = to,
        Subject = "Reset password",
        Body = body
    };
}
