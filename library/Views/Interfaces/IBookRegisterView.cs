// Presenter/Views/IBookRegisterView.cs
namespace library.Views.Interfaces;

public interface IBookRegisterView
{
    string BookName { get; }
    string Author { get; }
    string Publisher { get; }
    string Genre { get; }
    string ISBN { get; }

    void ShowError(string message);
    void NavigateToCompletion();                  // ← 引数なしに変更
    void Close();
}