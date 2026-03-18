using LibrarySystem.Model.DTOs;
using LibrarySystem.Model.Services;
using LibrarySystem.Presenter.Views;

namespace LibrarySystem.Presenter;

/// <summary>蔵書予約画面Presenter</summary>
public class ReservationPresenter
{
    private readonly IReservationView _view;
    private readonly IReservationService _reservationService;

    /// <summary>貸出画面から遷移した場合の引き継ぎ蔵書ID</summary>
    private readonly int? _prefilledBookId;

    public ReservationPresenter(IReservationView view, IReservationService reservationService,
        int? prefilledBookId = null)
    {
        _view = view;
        _reservationService = reservationService;
        _prefilledBookId = prefilledBookId;

        _view.ReserveClicked += async (s, e) => await OnReserveClickedAsync();
        _view.CancelClicked += (s, e) => _view.Close();
    }

    private async Task OnReserveClickedAsync()
    {
        try
        {
            if (!TryParseIds(out int userId, out int bookId, out string? parseError))
            {
                _view.ShowError(parseError!);
                return;
            }

            var result = await _reservationService.ReserveAsync(bookId, userId);

            if (!result.IsSuccess)
            {
                _view.ShowError(result.ErrorMessage!);
                return;
            }

            // 現在の返却期限日を表示（DateOnly? → IReservationViewへ渡す）
            _view.ShowReturnDue(result.Value!.CurrentReturnDue);

            _view.NavigateToCompletion(new CompletionViewModel { Message = "操作が完了しました" });
        }
        catch (Exception ex)
        {
            AppLogger.Error("予約処理中にエラーが発生しました", ex);
            _view.ShowError("システムエラーが発生しました。管理者へご連絡ください。");
        }
    }

    private bool TryParseIds(out int userId, out int bookId, out string? error)
    {
        userId = 0; bookId = 0; error = null;

        if (string.IsNullOrWhiteSpace(_view.UserId))
        {
            error = "利用者IDを入力してください。";
            return false;
        }
        if (!int.TryParse(_view.UserId, out userId))
        {
            error = "利用者IDは数値で入力してください。";
            return false;
        }

        // 引き継ぎIDがある場合はそちらを優先
        string bookIdStr = _prefilledBookId.HasValue
            ? _prefilledBookId.Value.ToString()
            : _view.BookId;

        if (string.IsNullOrWhiteSpace(bookIdStr))
        {
            error = "蔵書IDを入力してください。";
            return false;
        }
        if (!int.TryParse(bookIdStr, out bookId))
        {
            error = "蔵書IDは数値で入力してください。";
            return false;
        }
        return true;
    }
}
