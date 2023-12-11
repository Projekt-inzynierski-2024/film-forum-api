﻿using FilmForumWebAPI.Services.Interfaces;
using OtpNet;
using QRCoder;
using SimpleBase;
using System.Text;

namespace FilmForumWebAPI.Services;

public class MultifactorAuthenticationService : IMultifactorAuthenticationService
{
    private readonly QRCodeGenerator _qrCoderGenerator = new();

    private static string Secret(string unique) => $"SECRETKEY{unique}";

    public async Task<bool> VerifyCodeAsync(string email, string code) => await Task.Run(() =>
    {
        byte[] secretBytes = Encoding.ASCII.GetBytes(Secret(email));
        Totp totp = new(secretBytes, mode: OtpHashMode.Sha512);

        return totp.VerifyTotp(code, out _);
    });

    public async Task<string> GenerateUriAsync(string email) => await Task.Run(() =>
    {
        byte[] secretBytes = Encoding.ASCII.GetBytes(Secret(email));
        string base32 = Base32.Rfc4648.Encode(secretBytes);
        return $"otpauth://totp/FilmForum:{email}?secret={base32}&issuer=FilmForum&algorithm=SHA512";
    });

    public async Task<byte[]> GenerateQRCodePNGAsync(string text) => await Task.Run(() =>
    {
        QRCodeData qrData = _qrCoderGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
        return new PngByteQRCode(qrData).GetGraphic(5);
    });
}