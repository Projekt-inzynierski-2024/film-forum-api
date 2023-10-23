namespace FilmForumWebAPI.Models.Dtos;

public record CreateUserDto(string Username, string Email, string Password, string ConfirmPassword);

