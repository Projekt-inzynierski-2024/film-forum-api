using FilmForumModels.Models.Exceptions.Messages;

namespace FilmForumModels.Models.Exceptions;

public class InvalidRoleNameException : Exception
{
    public InvalidRoleNameException() : base(ExceptionsMessages.InvalidRoleName)
    {
    }
}