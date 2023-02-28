using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uamazing.SME.Server.Database
{
    public class LiteDBManager : LiteRepository
    {
        /// <summary>
        /// 数据库操作
        /// </summary>
        public LiteDBManager(string liteDbPath) : base(new ConnectionString()
            {
                Filename = liteDbPath,
                Upgrade = true
            }, new BsonMapper() { }.UseCamelCase()
        )
        {
        }
    }
}
