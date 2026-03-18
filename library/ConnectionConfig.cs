namespace library;

/// <summary>
/// 接続文字列を一箇所で管理する静的クラス。
/// Program.cs の起動時に Initialize() を呼び、
/// 各FormのLoadでは ConnectionConfig.ConnectionString を参照する。
/// </summary>
public static class ConnectionConfig
{
    public static string ConnectionString { get; private set; } = string.Empty;

    public static void Initialize(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("接続文字列が空です。appsettings.json を確認してください。");
        ConnectionString = connectionString;
    }
}