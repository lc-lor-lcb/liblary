using Microsoft.Data.SqlClient;
using System.Data;

namespace LibrarySystem.Infrastructure;

/// <summary>DB接続管理クラス</summary>
public class DatabaseConnectionFactory : IDisposable
{
    private readonly string _connectionString;

    public DatabaseConnectionFactory(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("接続文字列が設定されていません。", nameof(connectionString));
        _connectionString = connectionString;
    }

    /// <summary>新しいDB接続を開いて返す（呼び出し側でusingを使うこと）</summary>
    public IDbConnection CreateConnection()
    {
        var conn = new SqlConnection(_connectionString);
        conn.Open();
        return conn;
    }

    public void Dispose() { /* 接続プーリングはADO.NETに委ねる */ }
}

/// <summary>接続ファクトリのDI用インターフェース</summary>
public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}

/// <summary>実装ラッパー（DIコンテナ登録用）</summary>
public class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;
    public SqlConnectionFactory(string connectionString) => _connectionString = connectionString;

    public IDbConnection CreateConnection()
    {
        var conn = new SqlConnection(_connectionString);
        conn.Open();
        return conn;
    }
}
