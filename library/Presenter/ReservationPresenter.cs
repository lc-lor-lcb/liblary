using library.Model.Services;
using library.Views.Interfaces;  // ← 修正
using NLog;

namespace library.Presenter;

public class ReservationPresenter
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private readonly IReservationView _view;
    private readonly IReservationService _reservationService;
    private readonly ILoanService _loanService;
    private readonly IUserService _userService;
    private readonly IBookService _bookService;

    public ReservationPresenter(
        IReservationView view,
        IReservationService reservationService,
        ILoanService loanService,
        IUserService userService,
        IBookService bookService)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _reservationService = reservationService ?? throw new ArgumentNullException(nameof(reservationService));
        _loanService = loanService ?? throw new ArgumentNullException(nameof(loanService));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
    }

    /// <summary>蔵書IDが確定した時点で返却期限日を表示する。</summary>
    public async void OnBookIdConfirmed()
    {
        try
        {
            if (!int.TryParse(_view.BookId, out int bookId) || bookId <= 0)
                return;

            var log = await _loanService.GetActiveLoanByBookAsync(bookId);
            _view.ShowReturnDueDate(log?.ReturnDue.ToDateTime(TimeOnly.MinValue));
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "返却期限日取得中にエラーが発生しました。");
        }
    }

    public async void OnReserveClicked()
    {
        try
        {
            if (!TryParseIds(out int userId, out int bookId))
                return;

            var user = await _userService.GetByIdAsync(userId);
            if (user == null || !user.IsActive)
            {
                _view.ShowError("利用者IDが見つかりません。受付にお問い合わせください。");
                return;
            }

            var book = await _bookService.GetByIdAsync(bookId);
            if (book == null)
            {
                _view.ShowError("蔵書IDが見つかりません。");
                return;
            }

            // 重複予約チェック（GetByBookAsync で当該利用者の予約を確認）
            var existing = await _reservationService.GetByBookAsync(bookId);
            if (existing != null && existing.UserId == userId)
            {
                _view.ShowError("この蔵書はすでに予約済みです。");
                return;
            }

            var result = await _reservationService.ReserveAsync(bookId, userId);
            if (!result.IsSuccess)
            {
                _view.ShowError(result.ErrorMessage ?? "予約処理に失敗しました。");
                return;
            }

            Logger.Info("予約成功: UserId={UserId}, BookId={BookId}", userId, bookId);
            _view.NavigateToCompletion();
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "予約処理中にシステムエラーが発生しました。");
            _view.ShowError("システムエラーが発生しました。管理者へご連絡ください。");
        }
    }

    private bool TryParseIds(out int userId, out int bookId)
    {
        userId = 0; bookId = 0;

        if (string.IsNullOrWhiteSpace(_view.UserId))
        { _view.ShowError("利用者IDを入力してください。"); return false; }
        if (!int.TryParse(_view.UserId, out userId) || userId <= 0)
        { _view.ShowError("利用者IDは正の整数で入力してください。"); return false; }

        if (string.IsNullOrWhiteSpace(_view.BookId))
        { _view.ShowError("蔵書IDを入力してください。"); return false; }
        if (!int.TryParse(_view.BookId, out bookId) || bookId <= 0)
        { _view.ShowError("蔵書IDは正の整数で入力してください。"); return false; }

        return true;
    }
}