namespace FilmForumWebAPI.Services.Interfaces;

public interface IMultifactorAuthenticationService
{
    Task<bool> VerifyCodeAsync(string email, string code);

    Task<string> GenerateUriAsync(string email);

    Task<byte[]> GenerateQRCodePNGAsync(string text);
}