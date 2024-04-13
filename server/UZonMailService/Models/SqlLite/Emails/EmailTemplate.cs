using UZonMailService.Models.SqlLite.Base;
using UZonMailService.Models.SqlLite.UserInfos;

namespace UZonMailService.Models.SqlLite.Emails
{
    /// <summary>
    /// 邮件模板
    /// </summary>
    public class EmailTemplate:SqlId
    {
        public int UserId { get; set; }
        public User User { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 模板内容
        /// </summary>
        public string Content { get; set; }
    }
}
