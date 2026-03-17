namespace library.Views.Interfaces;

public interface ICompletionView
{
    event EventHandler? BackToTopClicked;
    void StartCountdown(int seconds);
    void NavigateToTop();
}