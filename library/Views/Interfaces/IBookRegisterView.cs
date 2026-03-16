using System.Collections.Generic;
using System.Text;
using System;

namespace library.Model.Entities;
public interface IBookRegisterView
{
    string BookName { get; }
    string Author { get; }
    string Publisher { get; }
    string Genre { get; }
    string ISBN { get; }

    event EventHandler RegisterClicked;
    event EventHandler CancelClicked;

    void ShowError(string message);
    void NavigateToCompletion();
}