using System;
using System.Collections.Generic;
using System.Text;

namespace library.Model.Entities;

/// <summary>
/// 利用者エンティティ
/// </summary>
public class User
{
    /// <summary>利用者ID（自動採番）</summary>
    public int Id { get; set; }

    /// <summary>氏名</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>性別（false:男性 / true:女性）</summary>
    public bool Gender { get; set; }

    /// <summary>生年月日</summary>
    public DateOnly Birth { get; set; }

    /// <summary>メールアドレス（UNIQUE）</summary>
    public string Mail { get; set; } = string.Empty;

    /// <summary>電話番号</summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>住所</summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>有効フラグ（false=論理削除済み）</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>作成日時</summary>
    public DateTime CreatedAt { get; set; }
}