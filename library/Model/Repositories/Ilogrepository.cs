// Model/Repositories/ILogRepository.cs
using library.Model.Entities;

namespace library.Model.Repositories;

public interface ILogRepository
{
    // --- 既存メソッド（変更なし・そのまま残す） ---
    Task<long> InsertAsync(Log log);
    Task<Log?> GetActiveLoanAsync(int bookId, int userId);
    Task<IList<Log>> GetActiveLoansAsync(int userId);
    Task<int> GetActiveLoanCountAsync(int userId);
    Task<bool> HasOverdueAsync(int userId);
    Task ReturnAsync(long logId, DateTime returnDate);
    Task<DateOnly?> GetReturnDueByBookIdAsync(int bookId);  // 既存なら残す

    // ★ 追加
    Task<Log?> GetActiveLoanByBookAsync(int bookId);
}