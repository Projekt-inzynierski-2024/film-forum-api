using FilmForumWebAPI.Services.Interfaces;
using OtpNet;
using QRCoder;
using SimpleBase;
using System.Text;

namespace FilmForumWebAPI.Services;

public class MultifactorAuthenticationService : IMultifactorAuthenticationService
{
    private readonly QRCodeGenerator _qrCoderGenerator = new();

    private static string Secret(string unique) => $"SECRETKEY{unique}";
    private static string MultifactorUri(string email, string secret) => $"otpauth://totp/FilmForum:{email}?secret={secret}&issuer=FilmForum&algorithm=SHA512";

    /// <summary>
    /// Verify the <paramref name="code"/> taking into account <paramref name="email"/> to pass multifactor authentication
    /// </summary>
    /// <param name="email">User's email</param>
    /// <param name="code">Code to validate</param>
    /// <returns>True if <paramref name="code"/> is validated successfully, otherwise false</returns>
    public async Task<bool> VerifyCodeAsync(string email, string code) => await Task.Run(() =>
    {
        byte[] secretBytes = Encoding.ASCII.GetBytes(Secret(email));
        Totp totp = new(secretBytes, mode: OtpHashMode.Sha512);
        return totp.VerifyTotp(code, out _);
    });

    /// <summary>
    /// Generates uri for multifactor authentication
    /// </summary>
    /// <param name="email">User's email</param>
    /// <returns>Uri for multifactor authentication</returns>
    public async Task<string> GenerateUriAsync(string email) => await Task.Run(() =>
    {
        byte[] secretBytes = Encoding.ASCII.GetBytes(Secret(email));
        string base32 = Base32.Rfc4648.Encode(secretBytes);
        return MultifactorUri(email, base32);
    });

    /// <summary>
    /// Generates a new code for multifactor authentication
    /// </summary>
    /// <param name="text">Text to be converted to QR</param>
    /// <returns>QR code for multifactor authentication</returns>
    public async Task<byte[]> GenerateQRCodePNGAsync(string text) => await Task.Run(() =>
    {
        QRCodeData qrData = _qrCoderGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
        return new PngByteQRCode(qrData).GetGraphic(5);
    });
}