using Microsoft.EntityFrameworkCore;
using UZonMailService.Models.LiteDB;
using UZonMailService.Models.SqlLite.Base;
using UZonMailService.Models.SqlLite.Permission;

namespace UZonMailService.Models.SqlLite.UserInfos
{
    /// <summary>
    /// 用户上下文
    /// </summary>
    public class User: SqlId
    {
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

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

        /// <summary>
        /// 用户角色
        /// 导航属性
        /// </summary>
        public List<UserRole> UserRoles { get; set; }
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
