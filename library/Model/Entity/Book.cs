using System;
using System.Collections.Generic;
using System.Text;

namespace LibrarySystem.Model.Entities;

/// <summary>
/// 蔵書ステータス
/// </summary>
public enum BookStatus : byte
{
    /// <summary>在庫あり</summary>
    Available = 0,

    /// <summary>貸出中</summary>
    Loaned = 1,

    /// <summary>予約済</summary>
    Reserved = 2,

    /// <summary>除籍</summary>
    Discarded = 3,
}

/// <summary>
/// 蔵書エンティティ
/// </summary>
public class Book
{
    /// <summary>蔵書ID（自動採番）</summary>
    public int Id { get; set; }

    /// <summary>図書名</summary>
    public string BookName { get; set; } = string.Empty;

    /// <summary>著者名</summary>
    public string Author { get; set; } = string.Empty;

    /// <summary>出版社</summary>
    public string Publisher { get; set; } = string.Empty;

    /// <summary>ジャンル</summary>
    public string Genre { get; set; } = string.Empty;

    /// <summary>ISBN番号（UNIQUE）</summary>
    public string Isbn { get; set; } = string.Empty;

    /// <summary>
    /// 蔵書ステータス
    /// （0:在庫 / 1:貸出中 / 2:予約済 / 3:除籍）
    /// </summary>
    public BookStatus Status { get; set; } = BookStatus.Available;

    /// <summary>
    /// 貸出中フラグ（true=貸出中）
    /// ※Status と冗長だが可読性向上のため保持。更新時は Status と整合性を保つこと。
    /// </summary>
    public bool IsLoaned { get; set; } = false;

    /// <summary>
    /// 予約済みフラグ（true=予約あり）
    /// ※Status と冗長だが可読性向上のため保持。更新時は Status と整合性を保つこと。
    /// </summary>
    public bool IsReserved { get; set; } = false;
}