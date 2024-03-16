using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uamazing.UZonEmail.Server.Config.SubConfigs;

namespace Uamazing.UZonEmail.Server.Database
{
    /// <summary>
    /// LiteDB管理器
    /// </summary>
    /// <remarks>
    /// 数据库操作
    /// </remarks>
    public class LiteDBContext(IConfiguration configuration) : LiteRepository(new ConnectionString()
        {
            Filename = configuration.GetValue<string>(DatabaseConfig.GetLiteDbPathConfigKey()),
            Upgrade = true
        }, new SMEBsonMapper())
    {
    }
}
