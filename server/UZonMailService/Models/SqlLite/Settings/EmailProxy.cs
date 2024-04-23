using UZonMailService.Models.SqlLite.Base;

namespace UZonMailService.Models.SqlLite.Settings
{
    /// <summary>
    /// 邮件代理
    /// </summary>
    public class EmailProxy : SqlId
    {
        /// <summary>
        /// 优先级
        /// 值越大，优先级越高，越先匹配
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 邮件匹配规则
        /// 使用正则表达式
        /// </summary>
        public string EmailMatch { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否生效
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 主机地址
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 密码，此处密码需要加密
        /// </summary>
        public string? Password { get; set; }
    }
}
