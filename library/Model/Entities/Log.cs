namespace LibrarySystem.Model.Entities;

/// <summary>貸出ログエンティティ</summary>
public class Log
{
    public long ID { get; set; }
    public int User_id { get; set; }
    public int Book_id { get; set; }
    public DateTime LoanDate { get; set; }
    public DateOnly ReturnDue { get; set; }
    public DateTime? ReturnDate { get; set; }

    /// <summary>返却済みかどうか</summary>
    public bool IsReturned => ReturnDate.HasValue;
}

/// <summary>予約ステータス</summary>
public enum ReservationStatus : byte
{
    Active = 0,   // 有効
    Fulfilled = 1, // 貸出済
    Cancelled = 2  // キャンセル
}

/// <summary>予約エンティティ</summary>
public class Reservation
{
    public int ID { get; set; }
    public int User_id { get; set; }
    public int Book_id { get; set; }
    public DateTime ReservationDate { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Active;
    public bool Notified { get; set; }
}
