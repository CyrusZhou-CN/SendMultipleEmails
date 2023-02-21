﻿using LiteDB;
using Server.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database
{
    public class LiteDBManager : LiteRepository
    {
        private UserConfig _config;
        /// <summary>
        /// 数据库操作
        /// </summary>
        public LiteDBManager(UserConfig config) : base(new ConnectionString()
            {
                Filename = config.LiteDbPath,
                Upgrade = true
            }, new BsonMapper() { }.UseCamelCase()
        )
        {
            _config = config;
        }
    }
}
