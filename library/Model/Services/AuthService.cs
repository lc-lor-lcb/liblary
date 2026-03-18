using LibrarySystem.Infrastructure;
using LibrarySystem.Model.Entities;
using LibrarySystem.Model.Repositories;

namespace LibrarySystem.Model.Services;

public class AuthService : IAuthService
{
    private readonly ILibrarianRepository _librarianRepo;
    private readonly SessionManager _session;

    public AuthService(ILibrarianRepository librarianRepo, SessionManager session)
    {
        _librarianRepo = librarianRepo;
        _session = session;
    }

    public async Task<Result<Librarian>> LoginAsync(string userName, string password)
    {
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            return Result<Librarian>.Fail("IDまたはパスワードが正しくありません");

        var librarian = await _librarianRepo.GetByUserNameAsync(userName);

        // ユーザーが存在しない・無効・パスワード不一致を同一メッセージで返す（列挙型攻撃対策）
        if (librarian == null || !librarian.IsActive || !PasswordHasher.Verify(password, librarian.Password))
            return Result<Librarian>.Fail("IDまたはパスワードが正しくありません");

        _session.Login(librarian);
        return Result<Librarian>.Ok(librarian);
    }

    public void Logout() => _session.Logout();
}
