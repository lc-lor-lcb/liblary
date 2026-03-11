using System;
using System.Collections.Generic;
using System.Text;
using LibrarySystem.Model.Services;
using LibrarySystem.Presenter.Views;
using NLog;

namespace LibrarySystem.Presenter;

/// <summary>
/// 蔵書予約画面のPresenter（利用者が自己操作）。
/// 予約登録・返却予定日表示・完了画面遷移を担当する。
/// </summary>
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

    /// <summary>
    /// 画面表示時（蔵書IDが確定した後）に呼び出す。
    /// 現在の貸出期限日を取得して画面に表示する。
    /// </summary>
    public void OnBookIdConfirmed()
    {
        try
        {
            if (!int.TryParse(_view.BookId, out int bookId) || bookId <= 0)
                return;

            // 現在の貸出期限日を取得して表示
            var currentLog = _loanService.GetActiveLoanByBook(bookId);
            _view.ShowReturnDueDate(currentLog?.ReturnDue.ToDateTime(TimeOnly.MinValue));
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "返却期限日取得中にエラーが発生しました。");
        }
    }

    /// <summary>
    /// 予約ボタン押下時に呼び出す。
    /// </summary>
    public void OnReserveClicked()
    {
        try
        {
            // --- 入力バリデーション ---
            if (!TryParseIds(out int userId, out int bookId))
                return;

            // --- 利用者存在確認・IsActiveチェック ---
            var user = _userService.GetById(userId);
            if (user == null || !user.IsActive)
            {
                _view.ShowError("利用者IDが見つかりません。受付にお問い合わせください。");
                return;
            }

            // --- 蔵書存在確認 ---
            var book = _bookService.GetById(bookId);
            if (book == null)
            {
                _view.ShowError("蔵書IDが見つかりません。");
                return;
            }

            // --- 重複予約チェック ---
            var existingReservation = _reservationService.GetActiveReservation(bookId, userId);
            if (existingReservation != null)
            {
                _view.ShowError("この蔵書はすでに予約済みです。");
                return;
            }

            // --- 予約実行 ---
            var result = _reservationService.Reserve(bookId, userId);
            if (!result.IsSuccess)
            {
                _view.ShowError(result.ErrorMessage ?? "予約処理に失敗しました。");
                return;
            }

            Logger.Info("予約成功: UserId={UserId}, BookId={BookId}", userId, bookId);

            _view.NavigateToCompletion(new CompletionViewModel
            {
                CompletionType = CompletionType.Reservation
            });
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "予約処理中にシステムエラーが発生しました。");
            _view.ShowError("システムエラーが発生しました。管理者へご連絡ください。");
        }
    }

    private bool TryParseIds(out int userId, out int bookId)
    {
        userId = 0;
        bookId = 0;

        if (string.IsNullOrWhiteSpace(_view.UserId))
        {
            _view.ShowError("利用者IDを入力してください。");
            return false;
        }
        if (!int.TryParse(_view.UserId, out userId) || userId <= 0)
        {
            _view.ShowError("利用者IDは正の整数で入力してください。");
            return false;
        }

        if (string.IsNullOrWhiteSpace(_view.BookId))
        {
            _view.ShowError("蔵書IDを入力してください。");
            return false;
        }
        if (!int.TryParse(_view.BookId, out bookId) || bookId <= 0)
        {
            _view.ShowError("蔵書IDは正の整数で入力してください。");
            return false;
        }

        return true;
    }
}