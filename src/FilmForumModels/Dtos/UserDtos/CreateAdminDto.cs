namespace FilmForumModels.Dtos.UserDtos;

public record CreateAdminDto(string Username, string Email, string Password, string ConfirmPassword, string SecretKey) : CreateUserDto(Username, Email, Password, ConfirmPassword);