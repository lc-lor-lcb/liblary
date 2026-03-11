using System;
using System.Collections.Generic;
using System.Text;
using LibrarySystem.Model.Entities;
using LibrarySystem.Model.Services;
using LibrarySystem.Presenter.Views;
using NLog;

namespace LibrarySystem.Presenter;

/// <summary>
/// 蔵書貸し出し画面のPresenter（利用者が自己操作）。
/// 貸出可否チェック・上限確認・延滞チェック・貸出実行・完了/予約画面遷移を担当する。
/// </summary>
public class CheckoutPresenter
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private const int LoanLimit = 5;

    private readonly ICheckoutView _view;
    private readonly ILoanService _loanService;
    private readonly IBookService _bookService;
    private readonly IUserService _userService;

    public CheckoutPresenter(
        ICheckoutView view,
        ILoanService loanService,
        IBookService bookService,
        IUserService userService)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _loanService = loanService ?? throw new ArgumentNullException(nameof(loanService));
        _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    /// <summary>
    /// 貸出ボタン押下時に呼び出す。
    /// </summary>
    public void OnCheckoutClicked()
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

            // --- 蔵書ステータス確認 ---
            var book = _bookService.GetById(bookId);
            if (book == null)
            {
                _view.ShowError("蔵書IDが見つかりません。");
                return;
            }

            if (!CanCheckout(book, userId))
                return;

            // --- 貸出中件数上限チェック ---
            var activeLoans = _loanService.GetActiveLoans(userId);
            if (activeLoans.Count >= LoanLimit)
            {
                _view.ShowError($"貸出上限（{LoanLimit}冊）に達しています。返却後にご利用ください。");
                return;
            }

            // --- 延滞チェック（警告のみ、処理は続行可能） ---
            bool hasOverdue = activeLoans.Any(l => l.ReturnDue < DateOnly.FromDateTime(DateTime.Today));
            if (hasOverdue)
            {
                _view.ShowWarning("返却期限を超過している蔵書があります。早めの返却をお願いします。");
            }

            // --- 貸出実行 ---
            var result = _loanService.Checkout(bookId, userId);
            if (!result.IsSuccess)
            {
                _view.ShowError(result.ErrorMessage ?? "貸出処理に失敗しました。");
                return;
            }

            Logger.Info("貸出成功: UserId={UserId}, BookId={BookId}", userId, bookId);

            _view.NavigateToCompletion(new CompletionViewModel
            {
                CompletionType = CompletionType.Checkout
            });
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "貸出処理中にシステムエラーが発生しました。");
            _view.ShowError("システムエラーが発生しました。管理者へご連絡ください。");
        }
    }

    /// <summary>
    /// 貸出可否を判定し、不可の場合は適切なメッセージ表示または予約画面へ誘導する。
    /// </summary>
    /// <returns>貸出可能な場合 true</returns>
    private bool CanCheckout(Book book, int userId)
    {
        switch (book.Status)
        {
            case BookStatus.InStock:
                return true;

            case BookStatus.Loaned:
                _view.ShowError("この蔵書は現在貸出中です。");
                _view.NavigateToReservation(book.ID);
                return false;

            case BookStatus.Reserved:
                // 予約者本人ならば貸出可
                if (book.ReservedByUserId == userId)
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