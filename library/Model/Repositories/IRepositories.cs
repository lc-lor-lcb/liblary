using LibrarySystem.Model.DTOs;
using LibrarySystem.Model.Entities;

namespace LibrarySystem.Model.Repositories;

// ---- 蔵書リポジトリ ----
public interface IBookRepository
{
    Task<IList<Book>> SearchAsync(BookSearchCriteria criteria);
    Task<Book?> GetByIdAsync(int bookId);
    Task<Book?> GetByIsbnAsync(string isbn);
    Task<int> InsertAsync(Book book);
    Task UpdateStatusAsync(int bookId, BookStatus status, bool isLoaned, bool isReserved);
}

// ---- 貸出ログリポジトリ ----
public interface ILogRepository
{
    Task<Log?> GetActiveLoanAsync(int bookId, int userId);
    Task<IList<Log>> GetActiveLoansByUserAsync(int userId);
    Task<int> CountActiveLoansByUserAsync(int userId);
    Task<bool> HasOverdueLoanAsync(int userId);
    Task<long> InsertAsync(Log log);
    Task SetReturnDateAsync(long logId, DateTime returnDate);
    Task<DateOnly?> GetCurrentReturnDueAsync(int bookId);
}

// ---- 予約リポジトリ ----
public interface IReservationRepository
{
    Task<Reservation?> GetActiveByBookAsync(int bookId);
    Task<bool> ExistsActiveByUserAndBookAsync(int userId, int bookId);
    Task<int> InsertAsync(Reservation reservation);
    Task UpdateStatusAsync(int reservationId, ReservationStatus status);
    Task SetNotifiedAsync(int reservationId);
    Task FulfillByBookAndUserAsync(int bookId, int userId);
}

// ---- 利用者リポジトリ ----
public interface IUserRepository
{
    Task<User?> GetByIdAsync(int userId);
    Task<IList<User>> SearchByNameAsync(string name);
    Task<bool> ExistsByMailAsync(string mail, int? excludeUserId = null);
    Task<int> InsertAsync(User user);
    Task UpdateAsync(User user);
    Task SetActiveAsync(int userId, bool isActive);
}

// ---- 司書リポジトリ ----
public interface ILibrarianRepository
{
    Task<Librarian?> GetByUserNameAsync(string userName);
    Task<Librarian?> GetByIdAsync(int id);
}
