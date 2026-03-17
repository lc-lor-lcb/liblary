namespace library.Views.Interfaces;

public interface ILoginView
{
    string UserName { get; }
    string Password { get; }

    event EventHandler? LoginClicked;

    void ShowError(string message);
    void NavigateToMain();
}