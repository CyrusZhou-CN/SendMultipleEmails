using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Models
{
    /// <summary>
    /// 远程服务
    /// </summary>
    public class RemoteServer:AutoObjectId
    {
        /// <summary>
        /// 服务 Id
        /// 用于调用远程服务
        /// 每台机器有唯一的一个 Id
        /// </summary>
        public string ServerId { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}
