using System;
using System.Collections.Generic;
using System.Text;

namespace library.UI.Interfaces;

public interface IReturnView
{
    string UserId { get; }
    string BookId { get; }

    event EventHandler ReturnClicked;

    void ShowError(string message);
    void NavigateToCompletion();
}