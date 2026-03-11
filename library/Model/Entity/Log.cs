using System;
using System.Collections.Generic;
using System.Text;

namespace LibrarySystem.Model.Entities;

/// <summary>
/// 貸出ログエンティティ
/// </summary>
public class Log
{
    /// <summary>ログID（自動採番）</summary>
    public long Id { get; set; }

    /// <summary>利用者ID（FK → users）</summary>
    public int UserId { get; set; }

    /// <summary>蔵書ID（FK → books）</summary>
    public int BookId { get; set; }

    /// <summary>貸出日時</summary>
    public DateTime LoanDate { get; set; }

    /// <summary>返却期限日（貸出日 + 14日）</summary>
    public DateOnly ReturnDue { get; set; }

    /// <summary>
    /// 実返却日時
    /// null = 貸出中
    /// </summary>
    public DateTime? ReturnDate { get; set; }

    // ナビゲーションプロパティ（任意でロード）
    public User? User { get; set; }
    public Book? Book { get; set; }

    /// <summary>現在貸出中かどうか</summary>
    public bool IsActive => ReturnDate is null;

    /// <summary>延滞中かどうか</summary>
    public bool IsOverdue => IsActive && ReturnDue < DateOnly.FromDateTime(DateTime.Today);
}