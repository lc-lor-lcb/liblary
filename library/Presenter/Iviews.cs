namespace LibrarySystem.Presenter.Views;

// ---- ログイン画面 ----
public interface ILoginView
{
    string UserName { get; }
    string Password { get; }
    event EventHandler LoginClicked;
    void ShowError(string message);
    void NavigateToMain();
}

// ---- メインメニュー ----
public interface IMainView
{
    event EventHandler BookListMenuClicked;
    event EventHandler BookRegisterMenuClicked;
    event EventHandler UserManageMenuClicked;
    event EventHandler LogoutMenuClicked;
    void ShowLibrarianName(string name);
    void OpenBookList();
    void OpenBookRegister();
    void OpenUserManage();
    void NavigateToLogin();
}

// ---- 蔵書一覧・検索 ----
public interface IBookListView
{
    string SearchBookName { get; }
    string SearchAuthor { get; }
    string SearchPublisher { get; }
    string SearchGenre { get; }
    string SearchBookId { get; }
    IList<int> SelectedStatuses { get; }
    event EventHandler SearchClicked;
    event EventHandler PageNextClicked;
    event EventHandler PagePrevClicked;
    void ShowBooks(IList<Model.Entities.Book> books);
    void ShowError(string message);
    int CurrentPage { get; set; }
}

// ---- 蔵書新規登録 ----
public interface IBookRegisterView
{
    string BookName { get; }
    string Author { get; }
    string Publisher { get; }
    string Genre { get; }
    string ISBN { get; }
    event EventHandler RegisterClicked;
    event EventHandler CancelClicked;
    void ShowError(string message);
    void NavigateToCompletion(Model.DTOs.CompletionViewModel vm);
    void Close();
}

// ---- 貸出画面 ----
public interface ICheckoutView
{
    string UserId { get; }
    string BookId { get; }
    event EventHandler CheckoutClicked;
    void ShowError(string message);
    void ShowOverdueWarning();
    void NavigateToCompletion(Model.DTOs.CompletionViewModel vm);
    void NavigateToReservation(int bookId);
}

// ---- 返却画面 ----
public interface IReturnView
{
    string UserId { get; }
    string BookId { get; }
    event EventHandler ReturnClicked;
    void ShowError(string message);
    void NavigateToCompletion(Model.DTOs.CompletionViewModel vm);
}

// ---- 予約画面 ----
public interface IReservationView
{
    string UserId { get; }
    string BookId { get; }
    event EventHandler ReserveClicked;
    event EventHandler CancelClicked;
    void ShowError(string message);
    void ShowReturnDue(Model.Entities.DateOnly? returnDue);
    void NavigateToCompletion(Model.DTOs.CompletionViewModel vm);
    void Close();
}

// ---- 完了画面 ----
public interface ICompletionView
{
    event EventHandler BackToTopClicked;
    void ShowMessage(string message);
    void StartAutoReturnTimer(int seconds);
    void NavigateToTop();
}

// ---- 利用者管理画面 ----
public interface IUserManageView
{
    string SearchName { get; }
    // 入力フィールド
    string InputName { get; }
    bool InputGender { get; }
    string InputBirth { get; }
    string InputMail { get; }
    string InputPhone { get; }
    string InputAddress { get; }
    int? SelectedUserId { get; }

    event EventHandler SearchClicked;
    event EventHandler RegisterClicked;
    event EventHandler UpdateClicked;
    event EventHandler DeactivateClicked;

    void ShowUsers(IList<Model.Entities.User> users);
    void ShowUserDetail(Model.Entities.User user);
    void ShowError(string message);
    void ShowSuccess(string message);
    void ClearInputs();
}
