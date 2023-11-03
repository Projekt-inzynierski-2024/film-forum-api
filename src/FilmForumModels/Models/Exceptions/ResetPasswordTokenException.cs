using FilmForumModels.Models.Exceptions.Messages;

namespace FilmForumModels.Models.Exceptions;

public class ResetPasswordTokenException : Exception
{
    public ResetPasswordTokenException() : base(ExceptionsMessages.InvalidResetPasswordTokenLifetime)
    {
    }
}