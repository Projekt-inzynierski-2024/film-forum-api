namespace FilmForumModels.Dtos.UserDtos;

public record CreateUserDto(string Username, string Email, string Password, string ConfirmPassword);
