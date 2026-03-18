using LibrarySystem.Model.DTOs;
using LibrarySystem.Presenter.Views;

namespace LibrarySystem.Presenter;

/// <summary>操作完了画面Presenter（全操作共通）</summary>
public class CompletionPresenter
{
    private readonly ICompletionView _view;
    private const int AutoReturnSeconds = 30;

    public CompletionPresenter(ICompletionView view)
    {
        _view = view;
        _view.BackToTopClicked += (s, e) => _view.NavigateToTop();
    }

    /// <summary>画面初期化：メッセージ表示と自動戻りタイマー開始</summary>
    public void Initialize(CompletionViewModel vm)
    {
        _view.ShowMessage(vm.Message);
        _view.StartAutoReturnTimer(AutoReturnSeconds);
    }
}
