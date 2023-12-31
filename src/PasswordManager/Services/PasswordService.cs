﻿using PasswordManager.Interfaces;

namespace PasswordManager.Services;

public class PasswordService : IPasswordService
{
    /// <summary>
    /// Verifies that the hash of given password matches hashed password
    /// </summary>
    /// <param name="password">Unhashed password</param>
    /// <param name="hashedPassword">Hashed password</param>
    /// <returns>True if the passwords match, otherwise false</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="password"/> or <paramref name="hashedPassword"/> is null</exception>
    public bool VerifyPassword(string password, string hashedPassword) => BCrypt.Net.BCrypt.Verify(password, hashedPassword);

    /// <summary>
    /// Hashes password
    /// </summary>
    /// <param name="password">Unhashed password</param>
    /// <returns>Hashed password</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="password"/> is null</exception>
    public string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);
}