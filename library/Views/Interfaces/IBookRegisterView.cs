namespace library.Views.Interfaces;

public interface IBookRegisterView
{
    string BookName { get; }
    string Author { get; }
    string Publisher { get; }
    string Genre { get; }
    string ISBN { get; }

    void ShowError(string message);
    void NavigateToCompletion();
    void Close();
}