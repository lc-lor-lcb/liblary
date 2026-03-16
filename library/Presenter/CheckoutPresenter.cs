using library.Model.Entities;
using library.Model.Services;
using library.Presenter.Views;
using NLog;

namespace library.Presenter;

public class CheckoutPresenter
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    private const int LoanLimit = 5;

    private readonly ICheckoutView _view;
    private readonly ILoanService _loanService;
    private readonly IBookService _bookService;
    private readonly IUserService _userService;
    private readonly IReservationService _reservationService;

    public CheckoutPresenter(
        ICheckoutView view,
        ILoanService loanService,
        IBookService bookService,
        IUserService userService,
        IReservationService reservationService)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _loanService = loanService ?? throw new ArgumentNullException(nameof(loanService));
        _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _reservationService = reservationService ?? throw new ArgumentNullException(nameof(reservationService));
    }

    public async void OnCheckoutClicked()
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

            if (!await CanCheckoutAsync(book, userId))
                return;

            var activeLoans = await _loanService.GetActiveLoansAsync(userId);
            if (activeLoans.Count >= LoanLimit)
            {
                _view.ShowError($"貸出上限（{LoanLimit}冊）に達しています。返却後にご利用ください。");
                return;
            }

            bool hasOverdue = activeLoans.Any(l => l.ReturnDue < DateOnly.FromDateTime(DateTime.Today));
            if (hasOverdue)
                _view.ShowWarning("返却期限を超過している蔵書があります。早めの返却をお願いします。");

            var result = await _loanService.CheckoutAsync(bookId, userId);
            if (!result.IsSuccess)
            {
                _view.ShowError(result.ErrorMessage ?? "貸出処理に失敗しました。");
                return;
            }

            Logger.Info("貸出成功: UserId={UserId}, BookId={BookId}", userId, bookId);
            _view.NavigateToCompletion();
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "貸出処理中にシステムエラーが発生しました。");
            _view.ShowError("システムエラーが発生しました。管理者へご連絡ください。");
        }
    }

    private async Task<bool> CanCheckoutAsync(Book book, int userId)
    {
        switch (book.Status)
        {
            case BookStatus.Available:
                return true;

            case BookStatus.Loaned:
                _view.ShowError("この蔵書は現在貸出中です。");
                _view.NavigateToReservation(book.Id);
                return false;

            case BookStatus.Reserved:
                var reservation = await _reservationService.GetByBookAsync(book.Id);
                if (reservation != null && reservation.UserId == userId)
                    return true;
                _view.ShowError("この蔵書は他の方が予約中です。");
                return false;

            case BookStatus.Discarded:
                _view.ShowError("この蔵書は除籍されています。");
                return false;

            default:
                _view.ShowError("蔵書の状態が不明です。受付にお問い合わせください。");
                return false;
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