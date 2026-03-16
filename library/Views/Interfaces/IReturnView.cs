using System;
using System.Collections.Generic;
using System.Text;

namespace library.UI.Interfaces;

public interface IReservationView
{
    string UserId { get; }
    string BookId { get; }

    event EventHandler ReserveClicked;

    void ShowReturnDue(string returnDue);
    void ShowError(string message);
    void NavigateToCompletion();
}