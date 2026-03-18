using LibrarySystem.Model.Entities;

namespace LibrarySystem.Infrastructure;

/// <summary>司書ログインセッション管理（インメモリシングルトン）</summary>
public class SessionManager
{
    private static readonly SessionManager _instance = new();
    private SessionManager() { }
    public static SessionManager Instance => _instance;

    private Librarian? _currentLibrarian;
    private DateTime _lastActivity;
    private static readonly TimeSpan SessionTimeout = TimeSpan.FromMinutes(30);

    public Librarian? CurrentLibrarian => _currentLibrarian;
    public bool IsLoggedIn => _currentLibrarian != null && !IsSessionExpired();

    public void Login(Librarian librarian)
    {
        _currentLibrarian = librarian;
        _lastActivity = DateTime.Now;
    }

    public void Logout()
    {
        _currentLibrarian = null;
    }

    /// <summary>操作のたびに呼び出してセッションを延長する</summary>
    public void RefreshActivity()
    {
        if (_currentLibrarian != null)
            _lastActivity = DateTime.Now;
    }

    private bool IsSessionExpired()
    {
        return DateTime.Now - _lastActivity > SessionTimeout;
    }

    /// <summary>セッション有効チェック（期限切れの場合は自動ログアウト）</summary>
    public bool ValidateSession()
    {
        if (_currentLibrarian == null) return false;
        if (IsSessionExpired())
        {
            Logout();
            return false;
        }
        RefreshActivity();
        return true;
    }
}
