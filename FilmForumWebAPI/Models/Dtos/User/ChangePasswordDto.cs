namespace FilmForumWebAPI.Models.Dtos.User;

public record ChangePasswordDto(string NewPassword, string ConfirmPassword);
