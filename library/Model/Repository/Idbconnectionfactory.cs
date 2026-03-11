using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace LibrarySystem.Infrastructure;

/// <summary>
/// DB接続生成インターフェース
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// オープン済みの IDbConnection を返す
    /// </summary>
    IDbConnection Create();
}