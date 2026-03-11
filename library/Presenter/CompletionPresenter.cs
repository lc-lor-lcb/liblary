using System;
using System.Collections.Generic;
using System.Text;
using LibrarySystem.Presenter.Views;
using NLog;

namespace LibrarySystem.Presenter;

/// <summary>
/// 操作完了画面のPresenter（全操作共通）。
/// 完了メッセージ表示制御・自動タイムアウト・トップ画面への戻り制御を担当する。
/// </summary>
public class CompletionPresenter
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>自動でトップへ戻るまでの秒数</summary>
    private const int AutoReturnSeconds = 30;

    private readonly ICompletionView _view;
    private System.Threading.Timer? _autoReturnTimer;

    public CompletionPresenter(ICompletionView view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }

    /// <summary>
    /// 画面表示時に呼び出す。ViewModelをセットし自動返却タイマーを開始する。
    /// </summary>
    public void OnLoad(CompletionViewModel viewModel)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(viewModel);

            _view.SetViewModel(viewModel);
            StartAutoReturnTimer();

            Logger.Info("完了画面表示: CompletionType={CompletionType}", viewModel.CompletionType);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "完了画面初期化中にエラーが発生しました。");
        }
    }

    /// <summary>
    /// 「トップへ戻る」ボタン押下時に呼び出す。
    /// </summary>
    public void OnBackToTopClicked()
    {
        StopAutoReturnTimer();
        _view.NavigateToTop();
    }

    /// <summary>
    /// 画面クローズ時に呼び出す。タイマーを破棄する。
    /// </summary>
    public void OnFormClosed()
    {
        StopAutoReturnTimer();
        _autoReturnTimer?.Dispose();
        _autoReturnTimer = null;
    }

    private void StartAutoReturnTimer()
    {
        StopAutoReturnTimer();
        _autoReturnTimer = new System.Threading.Timer(
            _ =>
            {
                Logger.Info("完了画面：自動タイムアウトによりトップへ戻ります。");
                _view.NavigateToTop();
            },
            state: null,
            dueTime: TimeSpan.FromSeconds(AutoReturnSeconds),
            period: Timeout.InfiniteTimeSpan);
    }

    private void StopAutoReturnTimer()
    {
        _autoReturnTimer?.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
    }
}