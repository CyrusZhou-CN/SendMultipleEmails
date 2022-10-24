using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Models
{
    /// <summary>
    /// 系统设置
    /// 与用户层无关，主要记录系统的信息
    /// 该表中仅记录一条数据
    /// </summary>
    internal class SystemSetting
    {
        /// <summary>
        /// 数据库的版本信息
        /// 在程序启动时，需要检查数据库版本是否匹配
        /// </summary>
        public LiteDbVersion LiteDbVersion { get; set; }

        /// <summary>
        /// 当前发件服务的id
        /// </summary>
        public string ClientId { get; set; }= ObjectId.NewObjectId().ToString();
    }

    /// <summary>
    /// 数据库版本
    /// </summary>
    internal class LiteDbVersion
    {
        /// <summary>
        /// 主版本
        /// </summary>
        public int Major { get; set; } = 0;

        /// <summary>
        /// 小版本
        /// </summary>
        public int Minor { get; set; } = 0;

        /// <summary>
        /// 打包版本
        /// </summary>
        public int Patch { get; set; } = 0;
    }
}
