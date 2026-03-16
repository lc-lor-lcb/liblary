using System;
using System.Collections.Generic;
using System.Text;

namespace library.UI.Interfaces;

public interface ILoginView
{
    string UserName { get; }
    string Password { get; }

    event EventHandler LoginClicked;

    void ShowError(string message);
    void NavigateToMain();
}