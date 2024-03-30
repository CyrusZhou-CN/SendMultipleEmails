using LiteDB;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UZonMailService.Models.LiteDB
{
    /// <summary>
    /// 一封邮件
    /// </summary>
    public class AnEmail : LinkingUserId
    {
        /// <summary>
        /// 任务 id
        /// </summary>
        public string SendMailTaskId { get; set; }

        /// <summary>
        /// 模板 id
        /// 使用的哪个模板
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// 发件箱
        /// </summary>
        public string Outbox { get; set; }

        /// <summary>
        /// 冗余：发件人姓名
        /// </summary>
        public string SenderUserName { get; set; }

        /// <summary>
        /// 收件箱
        /// </summary>
        public string Inbox { get; set; }

        /// <summary>
        /// 冗余：收件人姓名
        /// </summary>
        public string ReceiverUserName { get; set; }

        /// <summary>
        /// 邮件主题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        public List<string> AttachmentIds { get; set; }

        /// <summary>
        /// 抄送人邮箱
        /// </summary>
        public List<string> CopyTo { get; set; }

        /// <summary>
        /// 邮件 html 内容
        /// </summary>
        public string Html { get; set; }

        /// <summary>
        /// 是否发送
        /// </summary>
        public bool IsSent { get; set; }

        /// <summary>
        /// 重试次数
        /// </summary>
        public int TryCount { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime SendDate { get; set; }

        public override string GetFilterString()
        {
            return base.GetFilterString();
        }
    }
}
