using LibrarySystem.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using LibrarySystem.Model.Entities;

namespace LibrarySystem.Model.Repositories;

/// <summary>
/// 利用者データアクセスインターフェース
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// 利用者IDで利用者を取得する
    /// </summary>
    Task<User?> GetByIdAsync(int userId);

    /// <summary>
    /// 氏名で部分一致検索する
    /// </summary>
    Task<IList<User>> SearchByNameAsync(string name);

    /// <summary>
    /// メールアドレスで利用者を取得する（重複チェック用）
    /// </summary>
    Task<User?> GetByMailAsync(string mail);

    /// <summary>
    /// 利用者を新規挿入する
    /// </summary>
    Task<int> InsertAsync(User user);

    /// <summary>
    /// 利用者情報を更新する
    /// </summary>
    Task UpdateAsync(User user);

    /// <summary>
    /// 利用者を論理削除する（IsActive = 0）
    /// </summary>
    Task DeactivateAsync(int userId);
}