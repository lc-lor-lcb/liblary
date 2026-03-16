using library.Model.Dto;
using library.Model.Services;
using library.Presenter.Views;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace library.Presenter;

/// <summary>
/// 蔵書新規登録画面のPresenter。
/// 入力バリデーション・ISBN重複確認・蔵書登録実行・完了画面遷移を担当する。
/// </summary>
public class BookRegisterPresenter
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private readonly IBookRegisterView _view;
    private readonly IBookService _bookService;

    public BookRegisterPresenter(IBookRegisterView view, IBookService bookService)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
    }

    /// <summary>
    /// 登録ボタン押下時に呼び出す。
    /// </summary>
    public void OnRegisterClicked()
    {
        try
        {
            if (!ValidateInputs())
                return;

            var dto = new BookRegisterDto
            {
                BookName = _view.BookName.Trim(),
                Author = _view.Author.Trim(),
                Publisher = _view.Publisher.Trim(),
                Genre = _view.Genre.Trim(),
                ISBN = _view.ISBN.Trim()
            };

            // ISBN重複チェック
            var duplicateResult = _bookService.ExistsByIsbn(dto.ISBN);
            if (duplicateResult)
            {
                _view.ShowError("入力されたISBNはすでに登録されています。");
                return;
            }

            var result = _bookService.Register(dto);
            if (!result.IsSuccess)
            {
                _view.ShowError(result.ErrorMessage ?? "蔵書の登録に失敗しました。");
                return;
            }

            Logger.Info("蔵書登録成功: BookId={BookId}, ISBN={ISBN}", result.Value!.ID, dto.ISBN);

            _view.NavigateToCompletion(new CompletionViewModel
            {
                CompletionType = CompletionType.BookRegister
            });
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "蔵書登録中にシステムエラーが発生しました。");
            _view.ShowError("システムエラーが発生しました。管理者へご連絡ください。");
        }
    }

    /// <summary>
    /// キャンセルボタン押下時に呼び出す。
    /// </summary>
    public void OnCancelClicked()
    {
        _view.Close();
    }

    private bool ValidateInputs()
    {
        if (string.IsNullOrWhiteSpace(_view.BookName))
        {
            _view.ShowError("図書名を入力してください。");
            return false;
        }
        if (_view.BookName.Trim().Length > 500)
        {
            _view.ShowError("図書名は500文字以内で入力してください。");
            return false;
        }

        if (string.IsNullOrWhiteSpace(_view.Author))
        {
            _view.ShowError("著者名を入力してください。");
            return false;
        }
        if (_view.Author.Trim().Length > 200)
        {
            _view.ShowError("著者名は200文字以内で入力してください。");
            return false;
        }

        if (string.IsNullOrWhiteSpace(_view.Publisher))
        {
            _view.ShowError("出版社を入力してください。");
            return false;
        }
        if (_view.Publisher.Trim().Length > 200)
        {
            _view.ShowError("出版社は200文字以内で入力してください。");
            return false;
        }

        if (string.IsNullOrWhiteSpace(_view.Genre))
        {
            _view.ShowError("ジャンルを入力してください。");
            return false;
        }
        if (_view.Genre.Trim().Length > 100)
        {
            _view.ShowError("ジャンルは100文字以内で入力してください。");
            return false;
        }

        if (string.IsNullOrWhiteSpace(_view.ISBN))
        {
            _view.ShowError("ISBNを入力してください。");
            return false;
        }

        var isbnDigits = _view.ISBN.Trim().Replace("-", "");
        if (!System.Text.RegularExpressions.Regex.IsMatch(isbnDigits, @"^\d{10}$|^\d{13}$"))
        {
            _view.ShowError("ISBNは10桁または13桁の数字で入力してください。");
            return false;
        }

        return true;
    }
}