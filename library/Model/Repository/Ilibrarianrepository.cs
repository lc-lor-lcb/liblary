using System;
using System.Collections.Generic;
using System.Text;
using LibrarySystem.Model.Entities;

namespace LibrarySystem.Model.Repositories;

/// <summary>
/// 司書データアクセスインターフェース
/// </summary>
public interface ILibrarianRepository
{
    /// <summary>
    /// ログインIDで司書を取得する（認証用）
    /// </summary>
    Task<Librarian?> GetByUserNameAsync(string userName);

    /// <summary>
    /// 司書IDで司書を取得する
    /// </summary>
    Task<Librarian?> GetByIdAsync(int librarianId);

    /// <summary>
    /// 司書を新規挿入する
    /// </summary>
    Task<int> InsertAsync(Librarian librarian);

    /// <summary>
    /// パスワードハッシュを更新する
    /// </summary>
    Task UpdatePasswordAsync(int librarianId, string passwordHash);

    /// <summary>
    /// 司書の有効フラグを更新する
    /// </summary>
    Task SetActiveAsync(int librarianId, bool isActive);
}