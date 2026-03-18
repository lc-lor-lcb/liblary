using LibrarySystem.Model.DTOs;
using LibrarySystem.Model.Entities;

namespace LibrarySystem.Model.Services;

public interface IAuthService
{
    Task<Result<Librarian>> LoginAsync(string userName, string password);
    void Logout();
}

public interface IBookService
{
    Task<IList<Book>> SearchAsync(BookSearchCriteria criteria);
    Task<Result<Book>> GetByIdAsync(int bookId);
    Task<Result<Book>> RegisterAsync(BookRegisterDto dto);
}

public interface ILoanService
{
    Task<Result<LoanResult>> CheckoutAsync(int bookId, int userId);
    Task<Result<ReturnResult>> ReturnAsync(int bookId, int userId);
    Task<IList<Log>> GetActiveLoansAsync(int userId);
}

public interface IReservationService
{
    Task<Result<ReservationResult>> ReserveAsync(int bookId, int userId);
    Task<Result<bool>> CancelReservationAsync(int reservationId);
    Task<Reservation?> GetActiveByBookAsync(int bookId);
}

public interface IUserService
{
    Task<Result<User>> GetByIdAsync(int userId);
    Task<Result<User>> CreateUserAsync(UserDto dto);
    Task<Result<User>> UpdateUserAsync(int userId, UserDto dto);
    Task<IList<User>> SearchAsync(string name);
    Task<Result<bool>> DeactivateAsync(int userId);
}
