namespace FilmForumModels.Dtos.UserDtos;

public record ResetPasswordDto(string Email, string Password, string ConfirmPassword, string ResetPasswordToken);
