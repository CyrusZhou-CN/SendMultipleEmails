using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelayServer.Models
{
    /// <summary>
    /// 连接历史
    /// </summary>
    internal class ConnectedHistory
    {
        /// <summary>
        /// 客户端的 Id
        /// </summary>
        public string? ClientId { get; set; }

        /// <summary>
        /// 中继次数
        /// 每次发送后加 1
        /// </summary>
        public double RelayCount { get; set; }

        /// <summary>
        /// 失败的次数
        /// 代理端发来的发件成功回执
        /// </summary>
        public double SuccessCount { get; set; }

        /// <summary>
        /// 最近一次访问时间
        /// </summary>
        public DateTime LastVisitDate { get; set; }
    }
}
