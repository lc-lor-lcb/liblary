using System;
using System.Collections.Generic;
using System.Text;

namespace LibrarySystem.Model.Entities;

/// <summary>
/// 予約ステータス
/// </summary>
public enum ReservationStatus : byte
{
    /// <summary>有効（予約中）</summary>
    Active = 0,

    /// <summary>貸出済（予約確定）</summary>
    Loaned = 1,

    /// <summary>キャンセル済</summary>
    Cancelled = 2,
}

/// <summary>
/// 予約エンティティ
/// </summary>
public class Reservation
{
    /// <summary>予約ID（自動採番）</summary>
    public int Id { get; set; }

    /// <summary>予約利用者ID（FK → users）</summary>
    public int UserId { get; set; }

    /// <summary>蔵書ID（FK → books）</summary>
    public int BookId { get; set; }

    /// <summary>予約日時</summary>
    public DateTime ReservationDate { get; set; }

    /// <summary>
    /// 予約ステータス
    /// （0:有効 / 1:貸出済 / 2:キャンセル）
    /// </summary>
    public ReservationStatus Status { get; set; } = ReservationStatus.Active;

    /// <summary>返却通知済フラグ（true=通知済み）</summary>
    public bool Notified { get; set; } = false;

    // ナビゲーションプロパティ（任意でロード）
    public User? User { get; set; }
    public Book? Book { get; set; }
}