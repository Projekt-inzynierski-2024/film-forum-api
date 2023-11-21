namespace FilmForumWebAPI.Services.Interfaces;

public interface IMultifactorAuthenticationService
{
    bool VerifyCode(string email, string code);
    string GenerateUri(string email);
    byte[] GenerateQRCodePNG(string text);
}
