using UZonMailService.Models.SqlLite.Base;
using UZonMailService.Models.SqlLite.Emails;
using UZonMailService.Models.SqlLite.Files;
using UZonMailService.Models.SqlLite.Templates;
using UZonMailService.Models.SqlLite.UserInfos;

namespace UZonMailService.Models.SqlLite.EmailSending
{
    /// <summary>
    /// 邮件项
    /// </summary>
    public class SendingItem:SqlId
    {
        /// <summary>
        /// 所属发送任务
        /// </summary>
        public int SendingTaskId { get; set; }
        public SendingTask SendingTask { get; set; }

        /// <summary>
        /// 所属用户
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 发件人
        /// </summary>
        public int OutBoxId { get; set; }
        public Outbox OutBox { get; set; }

        /// <summary>
        /// 实际发件人
        /// 由于是多线程发件，这个值只有发送后才能确定
        /// </summary>
        public string? FromEmail { get; set; }

        /// <summary>
        /// 实际收件人
        /// 直接记录邮箱，因为可能是用户手动输入的邮箱号
        /// </summary>
        public string ToEmail { get; set; }

        /// <summary>
        /// 邮件模板 Id
        /// 可以为 0，表示不使用模板
        /// </summary>
        public int EmailTemplateId { get; set; }

        /// <summary>
        /// 实际发送内容
        /// 将模板与数据进行合并后的内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 附件列表
        /// </summary>
        public List<FileUsage> Attachments { get; set; }

        #region 发送结果
        /// <summary>
        /// 状态
        /// </summary>
        public SendingItemStatus Status { get; set; }

        /// <summary>
        /// 失败的原因
        /// </summary>
        public string? FailedReason { get; set; }

        /// <summary>
        /// 重试次数
        /// </summary>
        public int TriedCount { get; set; }
        #endregion
    }
}
