namespace library.Views.Interfaces;

public interface ICheckoutView
{
    string UserId { get; }
    string BookId { get; }

    void ShowError(string message);
    void ShowWarning(string message);
    void NavigateToCompletion();
    void NavigateToReservation(int bookId);
}