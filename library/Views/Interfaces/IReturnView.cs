// Presenter/Views/IReturnView.cs
namespace library.Views.Interfaces;

public interface IReturnView
{
    string UserId { get; }
    string BookId { get; }

    void ShowError(string message);
    void NavigateToCompletion();                  // ← 引数なしに変更
}