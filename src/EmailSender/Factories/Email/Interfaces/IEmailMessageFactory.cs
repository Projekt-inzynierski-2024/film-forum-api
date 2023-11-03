using FilmForumModels.Models.Email;

namespace EmailSender.Factories.Email.Interfaces;

public interface IEmailMessageFactory
{
    IEmailMessage Create(string to, string? subject = null, string? body = null);
}