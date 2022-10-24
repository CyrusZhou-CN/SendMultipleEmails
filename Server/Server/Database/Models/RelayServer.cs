using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Models
{
    /// <summary>
    /// 中继服务器
    /// </summary>
    internal class RelayServer:AutoObjectId
    {
        /// <summary>
        /// 用户 Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 远程地址，包含端口
        /// </summary>
        public string RemoteAddress { get; set; }

        /// <summary>
        /// 远程描述
        /// </summary>
        public string Description { get; set; }
    }
}
