using System;
using System.Collections.Generic;
using System.Text;
namespace library.Model.Dto;

/// <summary>蔵書検索条件DTO</summary>
public class BookSearchCriteria
{
    public string? BookName { get; set; }
    public string? Author { get; set; }
    public string? Publisher { get; set; }
    public string? Genre { get; set; }
    public IList<byte>? Statuses { get; set; }
    public int? BookId { get; set; }
}

/// <summary>蔵書新規登録DTO</summary>
public class BookRegisterDto
{
    public string BookName { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
}

/// <summary>貸出結果DTO</summary>
public class LoanResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public DateOnly? ReturnDue { get; set; }
    public bool HasOverdue { get; set; }
}

/// <summary>返却結果DTO</summary>
public class ReturnResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public bool HasNextReservation { get; set; }
    public int? NextReservationUserId { get; set; }
}

/// <summary>予約結果DTO</summary>
public class ReservationResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public DateOnly? CurrentReturnDue { get; set; }
}

/// <summary>利用者新規登録DTO</summary>
public class CreateUserDto
{
    public string Name { get; set; } = string.Empty;
    public bool Gender { get; set; }
    public DateOnly Birth { get; set; }
    public string Mail { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

/// <summary>利用者更新DTO</summary>
public class UpdateUserDto
{
    public string Name { get; set; } = string.Empty;
    public bool Gender { get; set; }
    public DateOnly Birth { get; set; }
    public string Mail { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}