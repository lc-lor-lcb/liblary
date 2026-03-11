using System;
using System.Collections.Generic;
using System.Text;
using LibrarySystem.Model.Services;
using LibrarySystem.Presenter.Views;
using NLog;

namespace LibrarySystem.Presenter;

/// <summary>
/// 蔵書返却画面のPresenter（利用者が自己操作）。
/// 返却処理・予約確認表示・完了画面遷移を担当する。
/// </summary>
public class ReturnPresenter
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private readonly IReturnView _view;
    private readonly ILoanService _loanService;

    public ReturnPresenter(IReturnView view, ILoanService loanService)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _loanService = loanService ?? throw new ArgumentNullException(nameof(loanService));
    }

    /// <summary>
    /// 返却ボタン押下時に呼び出す。
    /// </summary>
    public void OnReturnClicked()
    {
        try
        {
            // --- 入力バリデーション ---
            if (!TryParseIds(out int userId, out int bookId))
                return;

            // --- 返却処理 ---
            var result = _loanService.Return(bookId, userId);
            if (!result.IsSuccess)
            {
                _view.ShowError(result.ErrorMessage ?? "返却処理に失敗しました。");
                return;
            }

            Logger.Info("返却成功: UserId={UserId}, BookId={BookId}, HasReservation={HasReservation}",
                userId, bookId, result.Value!.HasNextReservation);

            _view.NavigateToCompletion(new CompletionViewModel
            {
                CompletionType = CompletionType.Return
            });
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "返却処理中にシステムエラーが発生しました。");
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