using LiteDB;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Models
{
    public class SendItem : AutoObjectId
    {
        /// <summary>
        /// 发件类型
        /// </summary>
        public SendType SendType { get; set; } = SendType.Html | SendType.Local;

        // 历史id
        public string TaskId { get; set; }

        // 接收者信息
        public EmailInfo Receiver { get; set; }

        // 抄送人邮箱
        public List<EmailInfo> CopyToEmails { get; set; }

        // 邮件主题
        public string Subject { get; set; }

        // 邮件 html 内容
        public string HtmlContent { get; set; }

        // 待发附件
        public List<EmailAttachment> Attachments { get; set; }

        #region 发件状态
        /// <summary>
        /// 是否发送
        /// </summary>
        public bool IsSent { get; set; }

        // 发送者信息
        public EmailInfo Sender { get; set; }

        /// <summary>
        /// 发送成功的时间
        /// </summary>
        public DateTime SentDate { get; set; }

        /// <summary>
        /// 如果是通过或者给远程发送，此处记录其 Id
        /// 远程发件时，发件的机器Id
        /// </summary>
        public string RemoteServerId { get; set; }

        /// <summary>
        /// 尝试次数
        /// </summary>
        public int TryCount { get; set; }

        // 发送结束后的消息
        public string SentResultMessage { get; set; }
        #endregion

        public override string GetFilterString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(base.GetFilterString());
            builder.Append(Sender.Email);
            builder.Append(Sender.UserName);
            builder.Append(Receiver.Email);
            builder.Append(Receiver.UserName);
            builder.Append(Subject);
            builder.Append(HtmlContent);
            builder.Append(SentResultMessage);

            return builder.ToString();           
        }
    }

    /// <summary>
    /// 发送类型
    /// </summary
    [Flags]
    public enum SendType
    {
        /// <summary>
        /// 发送内容为 HTML
        /// </summary>
        Html = 1 << 0,

        /// <summary>
        /// 发送内容为图片
        /// </summary>
        ImageDataUrl = 1 << 1,

        /// <summary>
        /// 本机发送
        /// </summary>
        Local = 1 << 2,

        /// <summary>
        /// 远程机器发送
        /// </summary>
        SendByRemote = 1 << 3,

        /// <summary>
        /// 为远程发送
        /// 其它机器发送给本机进行发送
        /// </summary>
        SendForRemote = 1 << 4,
    }
}
