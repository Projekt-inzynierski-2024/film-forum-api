namespace FilmForumWebAPI.Models.Dtos.User;

public record CreateUserDto(string Username, string Email, string Password, string ConfirmPassword);

