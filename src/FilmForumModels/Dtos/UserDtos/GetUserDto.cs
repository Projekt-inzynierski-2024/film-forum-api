using FilmForumModels.Entities;

namespace FilmForumModels.Dtos.UserDtos;

public class GetUserDto
{
    public GetUserDto(User user)
    {
        Id = user.Id;
        Username = user.Username;
        Email = user.Email;
    }
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
}