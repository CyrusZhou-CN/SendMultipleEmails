using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uamazing.SME.Server.Models;

namespace Uamazing.SME.Server.Models
{
    /// <summary>
    /// 用户表
    /// </summary>
    public class User : LinkingUserId
    {
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// signalR 连接 id
        /// </summary>
        public string ConnectionId { get; set; }
    }
}
