namespace FilmForumModels.Models.Email;

public interface IEmailMessage
{
    public string To { get; set; }
    public string? Subject { get; set; }
    public string? Body { get; set; }
}
