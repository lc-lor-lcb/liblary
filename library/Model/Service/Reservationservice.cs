// Model/Services/IReservationService.cs
using library.Model.Dto;
using library.Model.Entities;
using library.Model.Repositories;

namespace library.Model.Services;

public interface IReservationService
{
    Task<ReservationResult> ReserveAsync(int bookId, int userId);
    Task<bool> CancelReservationAsync(int reservationId);
    Task<Reservation?> GetByBookAsync(int bookId);
}

public class ReservationService : IReservationService
{
    private readonly IUserRepository _userRepository;
    private readonly IBookRepository _bookRepository;
    private readonly ILogRepository _logRepository;
    private readonly IReservationRepository _reservationRepository;

    public ReservationService(
        IUserRepository userRepository,
        IBookRepository bookRepository,
        ILogRepository logRepository,
        IReservationRepository reservationRepository)
    {
        _userRepository = userRepository;
        _bookRepository = bookRepository;
        _logRepository = logRepository;
        _reservationRepository = reservationRepository;
    }

    public async Task<ReservationResult> ReserveAsync(int bookId, int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || !user.IsActive)
            return Fail("利用者IDが存在しないか、無効な利用者です。");

        var book = await _bookRepository.GetByIdAsync(bookId);
        if (book == null)
            return Fail("蔵書IDが存在しません。");

        // ★ int との比較を BookStatus enum に修正
        if (book.Status == BookStatus.Discarded)
            return Fail("この蔵書は除籍済みのため予約できません。");

        if (book.Status == BookStatus.Available)
            return Fail("この蔵書は現在在庫があります。予約不要です。");

        bool alreadyReserved = await _reservationRepository.ExistsActiveAsync(userId, bookId);
        if (alreadyReserved)
            return Fail("この蔵書は既に予約済みです。");

        // 現在の返却期限日取得
        DateOnly? currentReturnDue = null;
        if (book.IsLoaned)
        {
            var activeLog = await _logRepository.GetActiveLoanByBookAsync(bookId);
            currentReturnDue = activeLog?.ReturnDue;
        }

        await _reservationRepository.InsertAsync(new Reservation
        {
            UserId = userId,
            BookId = bookId,
            ReservationDate = DateTime.Now,
            Status = 0,
            Notified = false,
        });

        await _bookRepository.UpdateStatusAsync(
            bookId,
            status: (byte)BookStatus.Reserved,
            isLoaned: book.IsLoaned,
            isReserved: true);

        return new ReservationResult { IsSuccess = true, CurrentReturnDue = currentReturnDue };
    }

    public async Task<bool> CancelReservationAsync(int reservationId)
    {
        await _reservationRepository.UpdateStatusAsync(reservationId, status: 2);
        return true;
    }

    public async Task<Reservation?> GetByBookAsync(int bookId)
        => await _reservationRepository.GetActiveByBookIdAsync(bookId);

    private static ReservationResult Fail(string message) =>
        new() { IsSuccess = false, ErrorMessage = message };
}