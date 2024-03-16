using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Uamazing.UZonEmail.Server.Config.SubConfigs;

namespace Uamazing.UZonEmail.Server.Database
{
    /// <summary>
    /// SqLite 上下文
    /// </summary>
    internal class SQLiteContext(IConfiguration configuration) : DbContext
    {
        private readonly IConfiguration _configuration = configuration;

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite(_configuration.GetValue<string>(DatabaseConfig.GetSqliteConnectionStringConfigKey()));
        }
    }
}
