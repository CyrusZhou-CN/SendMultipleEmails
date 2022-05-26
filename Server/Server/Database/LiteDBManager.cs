using LiteDB;
using Server.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database
{
    /// <summary>
    /// 数据库
    /// </summary>
    public class LiteDBManager : LiteRepository
    {
        /// <summary>
        /// 数据库静态引用
        /// </summary>
        public static LiteDBManager Instance { get; private set; } 

        /// <summary>
        /// 数据库操作
        /// </summary>
        public LiteDBManager(UserConfig config) : base(new ConnectionString()
        {
            Filename = config.LiteDbPath,
            Upgrade = true
        })
        {
            Instance = this;
        }
    }
}
