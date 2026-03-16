using System;
using System.Collections.Generic;
using System.Text;

namespace library.UI.Interfaces;

public interface IMainView
{
    event EventHandler BookListClicked;
    event EventHandler BookRegisterClicked;
    event EventHandler UserManageClicked;
    event EventHandler LogoutClicked;

    void ShowLibrarianName(string name);
}