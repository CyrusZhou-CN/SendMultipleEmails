using UZonMailService.Models.SqlLite.Base;
using UZonMailService.Models.SqlLite.Emails;
using UZonMailService.Models.SqlLite.EntityConfigs.Attributes;
using UZonMailService.Models.SqlLite.UserInfos;

namespace UZonMailService.Models.SqlLite.Settings
{
    /// <summary>
    /// 邮件代理
    /// 包括用户代理和系统内部代理
    /// </summary>
    public class UserProxy : SqlId
    {
        /// <summary>
        /// 所属用户
        /// 若为 0,则表示为系统代理
        /// </summary>
        public int UserId { get; set; } 

        /// <summary>
        /// 代理名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 优先级
        /// 值越大，优先级越高，越先匹配
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 邮件匹配规则
        /// 使用正则表达式
        /// </summary>
        public string? MatchRegex { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 是否生效
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 代理设置
        /// </summary>
        public string Proxy { get; set; }
    }
}
