using System;
using System.Collections.Generic;
using System.Text;
using LibrarySystem.Model.Dto;
using LibrarySystem.Model.Entities;

namespace LibrarySystem.Model.Repositories;

/// <summary>司書リポジトリインターフェース</summary>
public interface ILibrarianRepository
{
    Task<Librarian?> FindByUserNameAsync(string userName);
    Task<Librarian?> FindByIdAsync(int id);
}

/// <summary>利用者リポジトリインターフェース</summary>
public interface IUserRepository
{
    Task<User?> FindByIdAsync(int id);
    Task<IList<User>> SearchByNameAsync(string name);
    Task<bool> ExistsMailAsync(string mail, int? excludeId = null);
    Task<User> InsertAsync(User user);
    Task<User> UpdateAsync(User user);
}

/// <summary>蔵書リポジトリインターフェース</summary>
public interface IBookRepository
{
    Task<Book?> FindByIdAsync(int id);
    Task<IList<Book>> SearchAsync(BookSearchCriteria criteria);
    Task<bool> ExistsISBNAsync(string isbn, int? excludeId = null);
    Task<Book> InsertAsync(Book book);
    Task UpdateStatusAsync(int bookId, byte status, bool isLoaned, bool isReserved);
}

/// <summary>貸出ログリポジトリインターフェース</summary>
public interface ILogRepository
{
    Task<Log?> FindActiveLoanAsync(int bookId, int userId);
    Task<int> CountActiveLoansAsync(int userId);
    Task<bool> HasOverdueAsync(int userId);
    Task<IList<Log>> GetActiveLoansByUserAsync(int userId);
    Task<Log> InsertAsync(Log log);
    Task ReturnAsync(long logId, DateTime returnDate);
}

/// <summary>予約リポジトリインターフェース</summary>
public interface IReservationRepository
{
    Task<Reservation?> FindActiveByBookAndUserAsync(int bookId, int userId);
    Task<Reservation?> FindOldestActiveByBookAsync(int bookId);
    Task<bool> ExistsActiveByBookAsync(int bookId);
    Task<Reservation> InsertAsync(Reservation reservation);
    Task UpdateStatusAsync(int reservationId, byte status);
    Task SetNotifiedAsync(int reservationId);
}