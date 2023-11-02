namespace FilmForumModels.Models.Password;

public struct PasswordResetTokenWithExpirationDate
{
    public required string Token { get; set; }

    public required DateTime ExpirationDate { get; set; }
}