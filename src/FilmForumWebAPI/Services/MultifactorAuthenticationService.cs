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

    private String secret(String unique) => $"SECRETKEY{unique}";

    public bool VerifyCode(String unique, String code)
    {
        byte[] secretBytes = Encoding.ASCII.GetBytes(secret(unique));
        var totp = new Totp(secretBytes, mode: OtpHashMode.Sha512);

        return totp.VerifyTotp(code, out _);
    }

    public string GenerateUri(String unique)
    {
        byte[] secretBytes = Encoding.ASCII.GetBytes(secret(unique));
        string base32 = Base32.Rfc4648.Encode(secretBytes);
        return $"otpauth://totp/FilmForum:filmforum.api?secret={base32}&issuer=FilmForum&algorithm=SHA512";
    }

    public byte[] GenerateQRCodePNG(String text) 
    {
        QRCodeData qrData = _qrCoderGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
        return new PngByteQRCode(qrData).GetGraphic(5);
    }

}
