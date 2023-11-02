using FilmForumModels.Models.Email;
using FilmForumWebAPI.Factories.Email.Interfaces;

namespace FilmForumWebAPI.Factories.Email;

public class UserCreatedAccountEmailMessageFactory : IEmailMessageFactory
{
    public IEmailMessage Create(string to, string? subject = null, string? body = null) => new EmailMessage
    {
        To = to,
        Subject = subject ?? "Account registered",
        Body = body ?? $"Welcome to FilmForum, you have been successfully signed up using email: {to}. We hope you have a good time"
    };
}
