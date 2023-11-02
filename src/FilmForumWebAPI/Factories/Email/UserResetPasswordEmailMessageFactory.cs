using FilmForumModels.Models.Email;
using FilmForumWebAPI.Factories.Email.Interfaces;

namespace FilmForumWebAPI.Factories.Email;

public class UserResetPasswordEmailMessageFactory : IEmailMessageFactory
{
    public IEmailMessage Create(string to, string? subject = null, string? body = null) => new EmailMessage
    {
        To = to,
        Subject = "Reset password",
        Body = body
    };
}
