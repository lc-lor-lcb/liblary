// Presenter/Views/ICheckoutView.cs
namespace library.Views.Interfaces;

public interface ICheckoutView
{
    string UserId { get; }
    string BookId { get; }

    void ShowError(string message);
    void ShowWarning(string message);
    void NavigateToCompletion();                  // ← 引数なしに変更
    void NavigateToReservation(int bookId);
}