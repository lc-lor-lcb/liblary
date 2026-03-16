using System;
using System.Collections.Generic;
using System.Text;

namespace library.UI.Interfaces;

public interface ICheckoutView
{
    string UserId { get; }
    string BookId { get; }

    event EventHandler CheckoutClicked;

    void ShowError(string message);
    void ShowWarning(string message);
    void NavigateToCompletion();
    void NavigateToReservation(int bookId);
}