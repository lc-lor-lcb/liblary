using System;
using System.Collections.Generic;
using System.Text;
using LibrarySystem.Model.Entities;
using LibrarySystem.Model.Repositories;
using System.Security.Cryptography;

namespace LibrarySystem.Model.Services;

/// <summary>司書認証・セッション管理サービスインターフェース</summary>
public interface IAuthService
{
    /// <summary>
    /// ログインIDとパスワードで司書を認証する。
    /// 成功時は Librarian を返す。失敗時は null を返す。
    /// </summary>
    Task<Librarian?> LoginAsync(string userName, string password);

    /// <summary>現在のセッションをログアウトする。</summary>
    void Logout();

    /// <summary>現在ログイン中の司書。未ログイン時は null。</summary>
    Librarian? CurrentLibrarian { get; }

    /// <summary>ログイン中かどうかを返す。</summary>
    bool IsLoggedIn { get; }
}

/// <summary>司書認証・セッション管理サービス実装</summary>
public class AuthService : IAuthService
{
    private readonly ILibrarianRepository _librarianRepository;

    // ---- PBKDF2定数（設計書5.1.2準拠） ----
    private const int SaltSize = 16;         // bytes
    private const int HashSize = 32;         // bytes
    private const int Iterations = 600_000;
    private const byte FormatVersion = 1;

    public Librarian? CurrentLibrarian { get; private set; }
    public bool IsLoggedIn => CurrentLibrarian != null;

    public AuthService(ILibrarianRepository librarianRepository)
    {
        _librarianRepository = librarianRepository;
    }

    /// <inheritdoc/>
    public async Task<Librarian?> LoginAsync(string userName, string password)
    {
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            return null;

        var librarian = await _librarianRepository.FindByUserNameAsync(userName);

        // レコードが存在しない・無効・パスワード不一致のいずれも同一メッセージ（列挙攻撃対策）
        if (librarian == null || !librarian.IsActive)
            return null;

        if (!VerifyPassword(password, librarian.Password))
            return null;

        CurrentLibrarian = librarian;
        return librarian;
    }

    /// <inheritdoc/>
    public void Logout()
    {
        CurrentLibrarian = null;
    }

    // ----------------------------------------------------------------
    // パスワード検証
    // フォーマット: Base64( version[1byte] + salt[16byte] + hash[32byte] )
    // ----------------------------------------------------------------
    private static bool VerifyPassword(string inputPassword, string storedHash)
    {
        try
        {
            var decoded = Convert.FromBase64String(storedHash);
            // version(1) + salt(16) + hash(32) = 49 bytes
            if (decoded.Length != 1 + SaltSize + HashSize)
                return false;

            // バージョンチェック（将来の移行用）
            if (decoded[0] != FormatVersion)
                return false;

            var salt = decoded[1..(1 + SaltSize)];
            var storedHashBytes = decoded[(1 + SaltSize)..];

            var inputHashBytes = DeriveKey(inputPassword, salt);

            // 定数時間比較（タイミング攻撃対策）
            return CryptographicOperations.FixedTimeEquals(inputHashBytes, storedHashBytes);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 新規司書登録時等に使用するパスワードハッシュ生成メソッド。
    /// AuthService内で完結させ、外部にsaltを漏らさない。
    /// </summary>
    public static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = DeriveKey(password, salt);

        var result = new byte[1 + SaltSize + HashSize];
        result[0] = FormatVersion;
        salt.CopyTo(result, 1);
        hash.CopyTo(result, 1 + SaltSize);

        return Convert.ToBase64String(result);
    }

    private static byte[] DeriveKey(string password, byte[] salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(HashSize);
    }
}