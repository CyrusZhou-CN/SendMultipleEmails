using Server.Database.Extensions;
using Server.Database.Models;
using SqlSugar;
using System;
using System.Linq;

namespace Server.Database
{
    public class SqlSugarDatabaseManager
    {
        private readonly SqlSugarClient _db;

        public SqlSugarDatabaseManager(DatabaseConfig config)
        {
            DbType dbType;
            switch (config.DatabaseType.ToLower())
            {
                case "mssql":
                    dbType = DbType.SqlServer;
                    break;
                case "mysql":
                    dbType = DbType.MySql;
                    break;
                case "sqlite":
                    dbType = DbType.Sqlite;
                    break;
                default:
                    throw new NotSupportedException($"Database type {config.DatabaseType} is not supported.");
            }

            _db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = config.ConnectionString,
                DbType = dbType,
                IsAutoCloseConnection = true,
                ConfigureExternalServices = new ConfigureExternalServices
                {
                    EntityService = (c, p) =>
                    {
                        if (dbType == DbType.MySql && (p.DataType.ToLower() == "varchar(max)" || p.DataType.ToLower() == "nvarchar(max)"))
                        {
                            p.DataType = "longtext";
                        }
                    }
                }
            });

            _db.Aop.OnError = (error) =>
            {
                Console.WriteLine(error);
                Console.WriteLine();
            };
            _db.Aop.OnLogExecuting = (sql, pars) =>
            {
                Console.WriteLine(sql + "\r\n" + _db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                Console.WriteLine();
            };
            try
            {
                _db.CodeFirst.InitTables(typeof(User));
                _db.CodeFirst.InitTables(typeof(Group));
                _db.CodeFirst.InitTables(typeof(HistoryGroup));
                _db.CodeFirst.InitTables(typeof(ReceiveBox));
                _db.CodeFirst.InitTables(typeof(SendBox));
                _db.CodeFirst.InitTables(typeof(SendItem));
                _db.CodeFirst.InitTables(typeof(Setting));
                _db.CodeFirst.InitTables(typeof(Template));

                var user = _db.FirstOrDefault<User>(m => m.userId == "admin");
                if (user == null)
                {
                    user = new User
                    {
                        userId = "admin",
                        password = "111111",
                    };
                    _db.Insert(user);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        public ISqlSugarClient Db => _db;
    }
}
