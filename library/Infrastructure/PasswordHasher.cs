using System.Security.Cryptography;

namespace LibrarySystem.Infrastructure;

/// <summary>
/// PBKDF2（HMACSHA256、600,000回反復）によるパスワードハッシュ化ユーティリティ。
/// 保存形式: Base64( version[1byte] + salt[16byte] + hash[32byte] )
/// </summary>
public static class PasswordHasher
{
    private const byte FormatVersion = 0x01;
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 600_000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    /// <summary>パスワードをハッシュ化して保存用文字列を返す</summary>
    public static string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password, salt, Iterations, Algorithm, HashSize);

        byte[] payload = new byte[1 + SaltSize + HashSize];
        payload[0] = FormatVersion;
        salt.CopyTo(payload, 1);
        hash.CopyTo(payload, 1 + SaltSize);

        return Convert.ToBase64String(payload);
    }

    /// <summary>入力パスワードと保存済みハッシュを検証する</summary>
    public static bool Verify(string password, string storedHash)
    {
        byte[] payload;
        try
        {
            payload = Convert.FromBase64String(storedHash);
        }
        catch
        {
            return false;
        }

        if (payload.Length != 1 + SaltSize + HashSize || payload[0] != FormatVersion)
            return false;

        byte[] salt = payload[1..(1 + SaltSize)];
        byte[] expectedHash = payload[(1 + SaltSize)..];

        byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
            password, salt, Iterations, Algorithm, HashSize);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}
