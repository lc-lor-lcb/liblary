using System;
using System.Collections.Generic;
using System.Text;

namespace library.UI.Interfaces;

public interface IBookListView
{
    string SearchTitle { get; }
    string SearchAuthor { get; }
    string SearchPublisher { get; }
    string SearchGenre { get; }
    string SearchBookId { get; }
    IReadOnlyList<int> SelectedStatuses { get; }

    event EventHandler SearchClicked;

    void ShowBooks(IEnumerable<BookListItemViewModel> books);
    void ShowError(string message);
}

public record BookListItemViewModel(
    int Id,
    string BookName,
    string Author,
    string Publisher,
    string Genre,
    string StatusLabel,
    string? ReturnDue,
    bool IsOverdue
);