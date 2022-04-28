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
        [BsonField("historyId")]
        public string HistoryId { get; set; }

        // 发送者信息
        [BsonField("senderName")]
        public string SenderName { get; set; }

        [BsonField("senderEmail")]
        public string SenderEmail { get; set; }

        // 接收者信息
        [BsonField("receiverName")]
        public string ReceiverName { get; set; }

        [BsonField("receiverEmail")]
        public string ReceiverEmail { get; set; }

        // 抄送人邮箱
        [BsonField("copyToEmails")]
        public List<string> CopyToEmails { get; set; }

        // 邮件主题
        [BsonField("subject")]
        public string Subject { get; set; }

        // 邮件 html 内容
        [BsonField("html")]
        public string Html { get; set; }

        // 进度信息
        [BsonField("index")]
        public int Index { get; set; }

        [BsonField("total")]
        public int Total { get; set; }

        // 生成成果
        [BsonField("sendMessage")]
        public string SendMessage { get; set; }

        [BsonField("isSent")]
        public bool IsSent { get; set; }

        // 尝试次数
        [BsonField("tryCount")]
        public int TryCount { get; set; }

        // 发送时间
        [BsonField("sendDate")]
        public DateTime SendDate { get; set; }

        // 发送格式
        [BsonField("sendItemType")]
        public SendItemType SendItemType { get; set; }

        /// <summary>
        /// 内容 url
        /// </summary>
        [BsonField("dataUrl")]
        public string DataUrl { get; set; }

        // 待发附件
        [BsonField("attachments")]
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
