using System;
using System.Collections.Generic;
using System.Text;

namespace library.UI.Interfaces;

public interface ICompletionView
{
    event EventHandler BackToTopClicked;

    void StartCountdown(int seconds);
    void NavigateToTop();
}