namespace LibrarySystem.Model
{

    /// <summary>業務処理結果ラッパー（例外を使わないエラー伝達）</summary>
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public string? ErrorMessage { get; }

        private Result(bool isSuccess, T? value, string? error)
        {
            IsSuccess = isSuccess;
            Value = value;
            ErrorMessage = error;
        }

        public static Result<T> Ok(T value) => new(true, value, null);
        public static Result<T> Fail(string message) => new(false, default, message);
    }
}
// ---------- DTOs ----------

namespace LibrarySystem.Model.DTOs
{

    /// <summary>蔵書検索条件</summary>
    public class BookSearchCriteria
    {
        public string? BookName { get; set; }
        public string? Author { get; set; }
        public string? Publisher { get; set; }
        public string? Genre { get; set; }
        public int? BookId { get; set; }
        public IList<Entities.BookStatus> Statuses { get; set; } = new List<Entities.BookStatus>();
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
    }

    /// <summary>蔵書登録DTO</summary>
    public class BookRegisterDto
    {
        public string BookName { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
    }

    /// <summary>利用者登録・更新DTO</summary>
    public class UserDto
    {
        public string Name { get; set; } = string.Empty;
        public bool Gender { get; set; }
        public DateOnly Birth { get; set; }
        public string Mail { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }

    /// <summary>貸出結果</summary>
    public class LoanResult
    {
        public int LogId { get; set; }
        public DateOnly ReturnDue { get; set; }
        public bool HasOverdueWarning { get; set; }
    }

    /// <summary>返却結果</summary>
    public class ReturnResult
    {
        public bool HasPendingReservation { get; set; }
        public int? ReservationUserId { get; set; }
    }

    /// <summary>予約結果</summary>
    public class ReservationResult
    {
        public int ReservationId { get; set; }
        public DateOnly? CurrentReturnDue { get; set; }
    }

    /// <summary>完了画面用ViewModel</summary>
    public class CompletionViewModel
    {
        public string Message { get; set; } = "操作が完了しました";
    }

    /// <summary>完了操作種別</summary>
    public enum CompletionType
    {
        Checkout,
        Return,
        Reservation,
        BookRegister,
        UserRegister
    }
}