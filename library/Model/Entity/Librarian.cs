using System;
using System.Collections.Generic;
using System.Text;

namespace LibrarySystem.Model.Entities;

/// <summary>
/// 司書エンティティ
/// </summary>
public class Librarian
{
    /// <summary>司書ID（自動採番）</summary>
    public int Id { get; set; }

    /// <summary>ログインID（UNIQUE）</summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>司書氏名</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>メールアドレス（UNIQUE、NULL許可）</summary>
    public string? Mail { get; set; }

    /// <summary>
    /// パスワード（PBKDF2ハッシュ値）
    /// フォーマット：Base64( version[1byte] + salt[16byte] + hash[32byte] )
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>有効フラグ（false=無効）</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>作成日時</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>更新日時</summary>
    public DateTime UpdatedAt { get; set; }
}