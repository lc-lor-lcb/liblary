using LibrarySystem.Model.DTOs;
using LibrarySystem.Model.Entities;
using LibrarySystem.Model.Repositories;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace LibrarySystem.Model.Services;

// ---- ReservationService ----
public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepo;
    private readonly IBookRepository _bookRepo;
    private readonly ILogRepository _logRepo;
    private readonly IUserRepository _userRepo;

    public ReservationService(IReservationRepository reservationRepo, IBookRepository bookRepo,
        ILogRepository logRepo, IUserRepository userRepo)
    {
        _reservationRepo = reservationRepo;
        _bookRepo = bookRepo;
        _logRepo = logRepo;
        _userRepo = userRepo;
    }

    public async Task<Result<ReservationResult>> ReserveAsync(int bookId, int userId)
    {
        // 利用者・蔵書存在確認
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null || !user.IsActive)
            return Result<ReservationResult>.Fail("利用者IDが見つかりません。");

        var book = await _bookRepo.GetByIdAsync(bookId);
        if (book == null)
            return Result<ReservationResult>.Fail("蔵書IDが見つかりません。");

        if (book.Status == BookStatus.InStock)
            return Result<ReservationResult>.Fail("この蔵書は在庫中です。直接貸出手続きを行ってください。");

        if (book.Status == BookStatus.Retired)
            return Result<ReservationResult>.Fail("この蔵書は除籍済みです。");

        // 重複予約チェック
        bool duplicate = await _reservationRepo.ExistsActiveByUserAndBookAsync(userId, bookId);
        if (duplicate)
            return Result<ReservationResult>.Fail("この蔵書はすでに予約済みです。");

        // 現在の返却期限を取得
        var currentReturnDue = await _logRepo.GetCurrentReturnDueAsync(bookId);

        // 予約登録
        var reservation = new Reservation
        {
            User_id = userId,
            Book_id = bookId,
            ReservationDate = DateTime.Now,
            Status = ReservationStatus.Active
        };
        int reservationId = await _reservationRepo.InsertAsync(reservation);

        // 蔵書ステータスを予約済みに更新
        await _bookRepo.UpdateStatusAsync(bookId, BookStatus.Reserved, isLoaned: book.IsLoaned, isReserved: true);

        return Result<ReservationResult>.Ok(new ReservationResult
        {
            ReservationId = reservationId,
            CurrentReturnDue = currentReturnDue
        });
    }

    public async Task<Result<bool>> CancelReservationAsync(int reservationId)
    {
        await _reservationRepo.UpdateStatusAsync(reservationId, ReservationStatus.Cancelled);
        return Result<bool>.Ok(true);
    }

    public Task<Reservation?> GetActiveByBookAsync(int bookId)
        => _reservationRepo.GetActiveByBookAsync(bookId);
}

// ---- UserService ----
public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;

    public UserService(IUserRepository userRepo) => _userRepo = userRepo;

    public async Task<Result<User>> GetByIdAsync(int userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        return user != null
            ? Result<User>.Ok(user)
            : Result<User>.Fail($"利用者ID {userId} は存在しません");
    }

    public async Task<Result<User>> CreateUserAsync(UserDto dto)
    {
        var validationError = ValidateDto(dto);
        if (validationError != null) return Result<User>.Fail(validationError);

        bool mailExists = await _userRepo.ExistsByMailAsync(dto.Mail);
        if (mailExists) return Result<User>.Fail("このメールアドレスは既に登録されています。");

        var user = DtoToUser(dto);
        user.ID = await _userRepo.InsertAsync(user);
        return Result<User>.Ok(user);
    }

    public async Task<Result<User>> UpdateUserAsync(int userId, UserDto dto)
    {
        var existing = await _userRepo.GetByIdAsync(userId);
        if (existing == null) return Result<User>.Fail("利用者が見つかりません。");

        var validationError = ValidateDto(dto);
        if (validationError != null) return Result<User>.Fail(validationError);

        bool mailExists = await _userRepo.ExistsByMailAsync(dto.Mail, excludeUserId: userId);
        if (mailExists) return Result<User>.Fail("このメールアドレスは既に使用されています。");

        var user = DtoToUser(dto);
        user.ID = userId;
        await _userRepo.UpdateAsync(user);
        return Result<User>.Ok(user);
    }

    public Task<IList<User>> SearchAsync(string name)
        => _userRepo.SearchByNameAsync(name);

    public async Task<Result<bool>> DeactivateAsync(int userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) return Result<bool>.Fail("利用者が見つかりません。");
        await _userRepo.SetActiveAsync(userId, false);
        return Result<bool>.Ok(true);
    }

    private static string? ValidateDto(UserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Length > 100)
            return "氏名を1〜100文字で入力してください。";
        if (dto.Birth >= DateOnly.FromDateTime(DateTime.Today))
            return "生年月日は過去の日付を入力してください。";
        if (!IsValidEmail(dto.Mail))
            return "メールアドレスの形式が正しくありません。";
        if (!Regex.IsMatch(dto.Phone, @"^[\d\-]{7,15}$"))
            return "電話番号は数字とハイフンのみ、7〜15桁で入力してください。";
        if (string.IsNullOrWhiteSpace(dto.Address) || dto.Address.Length > 500)
            return "住所を500文字以内で入力してください。";
        return null;
    }

    private static bool IsValidEmail(string mail)
    {
        try { _ = new MailAddress(mail); return true; }
        catch { return false; }
    }

    private static User DtoToUser(UserDto dto) => new()
    {
        Name = dto.Name,
        Gender = dto.Gender,
        Birth = dto.Birth,
        Mail = dto.Mail,
        Phone = dto.Phone,
        Address = dto.Address
    };
}
