using UZonMailService.Models.SqlLite.Base;
using UZonMailService.Models.SqlLite.Emails;
using UZonMailService.Models.SqlLite.UserInfos;

namespace UZonMailService.Models.SqlLite.EmailSending
{
    /// <summary>
    /// 发送任务
    /// </summary>
    public class SendingTask:SqlId
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public int UserId { get; set; }
        public User User { get; set; }

        /// <summary>
        /// 发件箱
        /// </summary>
        public List<Outbox> OutBoxes { get; set; }

        /// <summary>
        /// 收件箱
        /// </summary>
        public List<Inbox> InBoxes { get; set; }

        /// <summary>
        /// 发件模板
        /// </summary>
        public List<EmailTemplate> EmailTemplates { get; set; }

        /// <summary>
        /// 是否分布式发件
        /// </summary>
        public bool IsDistributed { get; set; }
    }
}
