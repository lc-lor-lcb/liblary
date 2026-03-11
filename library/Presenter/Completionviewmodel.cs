using System;
using System.Collections.Generic;
using System.Text;
namespace LibrarySystem.Presenter.Views;

public enum CompletionType
{
    Checkout,
    Return,
    Reservation,
    BookRegister,
    UserRegister,
    UserUpdate
}

public class CompletionViewModel
{
    public CompletionType CompletionType { get; set; }
    public string Message { get; } = "操作が完了しました";
}