namespace LibrarySystem.Model.Entities;

/// <summary>蔵書ステータス</summary>
public enum BookStatus : byte
{
    InStock = 0,    // 在庫
    Loaned = 1,     // 貸出中
    Reserved = 2,   // 予約済
    Retired = 3     // 除籍
}

/// <summary>蔵書エンティティ</summary>
public class Book
{
    public int ID { get; set; }
    public string BookName { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public BookStatus Status { get; set; } = BookStatus.InStock;
    public bool IsLoaned { get; set; }
    public bool IsReserved { get; set; }

    /// <summary>貸出中の場合の返却期限日（ビュー用補助フィールド）</summary>
    public DateOnly? ReturnDue { get; set; }
}
