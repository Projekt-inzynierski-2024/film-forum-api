using FilmForumModels.Models.Email;

namespace FilmForumWebAPI.Factories.Email.Interfaces;

public interface IEmailMessageFactory
{
    IEmailMessage Create(string to, string? subject = null, string? body = null);
}
