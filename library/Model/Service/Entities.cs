using System;
using System.Collections.Generic;
using System.Text;
namespace LibrarySystem.Model.Entities;

/// <summary>司書エンティティ</summary>
public class Librarian
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Mail { get; set; }
    public string Password { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>利用者エンティティ</summary>
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Gender { get; set; }   // false=男性, true=女性
    public DateOnly Birth { get; set; }
    public string Mail { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}

/// <summary>蔵書エンティティ</summary>
public class Book
{
    public int Id { get; set; }
    public string BookName { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;

    /// <summary>0:在庫 1:貸出中 2:予約済 3:除籍</summary>
    public byte Status { get; set; } = 0;
    public bool IsLoaned { get; set; } = false;
    public bool IsReserved { get; set; } = false;
}

/// <summary>貸出ログエンティティ</summary>
public class Log
{
    public long Id { get; set; }
    public int UserId { get; set; }
    public int BookId { get; set; }
    public DateTime LoanDate { get; set; }
    public DateOnly ReturnDue { get; set; }
    public DateTime? ReturnDate { get; set; }
}

/// <summary>予約エンティティ</summary>
public class Reservation
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int BookId { get; set; }
    public DateTime ReservationDate { get; set; }

    /// <summary>0:有効 1:貸出済 2:キャンセル</summary>
    public byte Status { get; set; } = 0;
    public bool Notified { get; set; } = false;
}