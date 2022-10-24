using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelayServer.Models
{
    /// <summary>
    /// 已经连接的客户端
    /// 非数据库定义
    /// </summary>
    internal class ConnectedClient
    {
        /// <summary>
        /// Id
        /// </summary>
        public string? Id { get;private set; } 

        /// <summary>
        /// 描述
        /// </summary>

        public string? Description { get; set; }
    }
}
