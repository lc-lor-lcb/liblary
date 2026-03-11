using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace LibrarySystem.Infrastructure;

/// <summary>
/// SQL Server 用 DB接続ファクトリ
/// 接続文字列は appsettings.json から DI で注入する。
/// </summary>
public sealed class SqlServerConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlServerConnectionFactory(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("接続文字列が設定されていません。", nameof(connectionString));

        _connectionString = connectionString;
    }

    /// <inheritdoc />
    public IDbConnection Create()
    {
        var conn = new SqlConnection(_connectionString);
        conn.Open();
        return conn;
    }
}