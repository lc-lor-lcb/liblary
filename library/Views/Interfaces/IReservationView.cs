namespace library.Views.Interfaces;

public interface IReservationView
{
    string UserId { get; }
    string BookId { get; }

    void ShowError(string message);
    void ShowReturnDueDate(DateTime? returnDue);
    void NavigateToCompletion();
}