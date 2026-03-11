Ousing System;
using System.Collections.Generic;
using System.Text;
using LibrarySystem.Model.Entities;

namespace LibrarySystem.Presenter.Views;

public interface ILoginView
{
    string UserName { get; }
    string Password { get; }
    void ShowError(string message);
    void NavigateToMain();
}

public interface IBookListView
{
    string SearchBookName { get; }
    string SearchAuthor { get; }
    string SearchPublisher { get; }
    string SearchGenre { get; }
    IEnumerable<int> SearchStatuses { get; }
    string SearchBookId { get; }
    void ShowBooks(IList<Book> books);
    void ShowError(string message);
}

public interface IBookRegisterView
{
    string BookName { get; }
    string Author { get; }
    string Publisher { get; }
    string Genre { get; }
    string ISBN { get; }
    void ShowError(string message);
    void NavigateToCompletion(CompletionViewModel viewModel);
    void Close();
}

public interface ICheckoutView
{
    string UserId { get; }
    string BookId { get; }
    void ShowError(string message);
    void ShowWarning(string message);
    void NavigateToCompletion(CompletionViewModel viewModel);
    void NavigateToReservation(int bookId);
}

public interface IReturnView
{
    string UserId { get; }
    string BookId { get; }
    void ShowError(string message);
    void NavigateToCompletion(CompletionViewModel viewModel);
}

public interface IReservationView
{
    string UserId { get; }
    string BookId { get; }
    void ShowReturnDueDate(DateTime? returnDue);
    void ShowError(string message);
    void NavigateToCompletion(CompletionViewModel viewModel);
}

public interface ICompletionView
{
    void SetViewModel(CompletionViewModel viewModel);
    void NavigateToTop();
}

public interface IUserManageView
{
    string SearchName { get; }
    string InputName { get; }
    bool InputGender { get; }
    DateTime InputBirth { get; }
    string InputMail { get; }
    string InputPhone { get; }
    string InputAddress { get; }
    int? SelectedUserId { get; }
    void ShowUsers(IList<User> users);
    void ShowError(string message);
    void ClearInputs();
}