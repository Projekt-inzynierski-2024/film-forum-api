namespace FilmForumModels.Models.Password;

public record struct ValidateResetPasswordTokenResult(bool IsValid, string Message);