namespace FilmForumWebAPI.Services.Interfaces;

public interface IPasswordService
{
    public bool VerifyPassword(string password, string hashedPassword);

    public string HashPassword(string password);
}
