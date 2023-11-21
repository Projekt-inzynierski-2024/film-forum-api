using System.Text;
using OtpNet;
using QRCoder;
using SimpleBase;
using FilmForumWebAPI.Services.Interfaces;

namespace FilmForumWebAPI.Services;

public class MultifactorAuthenticationService : IMultifactorAuthenticationService
{
    private readonly QRCodeGenerator _qrCoderGenerator;

    public MultifactorAuthenticationService() 
    {
        _qrCoderGenerator = new QRCodeGenerator();
    }

    private string secret(string unique) => $"SECRETKEY{unique}";

    public bool VerifyCode(string email, string code)
    {
        byte[] secretBytes = Encoding.ASCII.GetBytes(secret(email));
        var totp = new Totp(secretBytes, mode: OtpHashMode.Sha512);

        return totp.VerifyTotp(code, out _);
    }

    public string GenerateUri(string email)
    {
        byte[] secretBytes = Encoding.ASCII.GetBytes(secret(email));
        string base32 = Base32.Rfc4648.Encode(secretBytes);
        return $"otpauth://totp/FilmForum:{email}?secret={base32}&issuer=FilmForum&algorithm=SHA512";
    }

    public byte[] GenerateQRCodePNG(string text) 
    {
        QRCodeData qrData = _qrCoderGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
        return new PngByteQRCode(qrData).GetGraphic(5);
    }

}
