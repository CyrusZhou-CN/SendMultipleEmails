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
        // 历史id
       
        public string HistoryId { get; set; }

        // 发送者信息
        public string SenderName { get; set; }

        public string SenderEmail { get; set; }

        // 接收者信息
        public string ReceiverName { get; set; }

        public string ReceiverEmail { get; set; }

        // 抄送人邮箱
        public List<string> CopyToEmails { get; set; }

        // 邮件主题
        public string Subject { get; set; }

        // 邮件 html 内容
        public string Html { get; set; }

        // 进度信息
        public int Index { get; set; }

        public int Total { get; set; }

        // 生成成果
        public string SendMessage { get; set; }
        public bool IsSent { get; set; }

        // 尝试次数
        public int TryCount { get; set; }

        // 发送时间
        public DateTime SendDate { get; set; }

        // 发送格式
        public SendItemType SendItemType { get; set; }

        /// <summary>
        /// 内容 url
        /// </summary>
        public string DataUrl { get; set; }

        // 待发附件
        public List<EmailAttachment> Attachments { get; set; }

        public override string GetFilterString()
        {
            return base.GetFilterString() + SenderName + SenderEmail + ReceiverEmail + ReceiverEmail + Subject + SendMessage;
        }
    }

    public enum SendItemType
    {
        /// <summary>
        /// 无
        /// </summary>
        none,

        /// <summary>
        /// html格式
        /// </summary>
        html,

        /// <summary>
        /// 数据 URL
        /// </summary>
        dataUrl,
    }
}
