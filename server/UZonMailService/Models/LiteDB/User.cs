using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UZonMailService.Models.LiteDB
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

        /// <summary>
        /// 是否是管理员
        /// </summary>
        public bool IsAdmin { get; set; } = false;

        /// <summary>
        /// 账户状态
        /// </summary>
        public UserStatus Status { get; set; } = UserStatus.Normal;
    }

    /// <summary>
    /// 用户状态
    /// </summary>
    public enum UserStatus
    {
        /// <summary>
        /// 被删除
        /// </summary>
        Deleted,

        /// <summary>
        /// </summary>
        Normal,

        /// <summary>
        /// 不允许登陆
        /// </summary>
        NotAllowedToLogin
    }
}
