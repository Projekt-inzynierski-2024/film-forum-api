namespace FilmForumWebAPI.Services.Interfaces;

public interface IMultifactorAuthenticationService
{
    bool VerifyCode(String secret, String code);
    string GenerateUri(String secret);
    byte[] GenerateQRCodePNG(String text);
}
