using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary.Database.Models
{
    public class SendItem : AutoObjectId
    {
        // 历史id
        public string historyId { get; set; }

        // 发送者信息
        [SugarColumn(IsNullable = true)]
        public string senderName { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = StaticConfig.CodeFirst_BigString)]
        public string senderEmail { get; set; }

        // 接收者信息
        [SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString)]
        public string receiverName { get; set; }

        [SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString)]
        public string receiverEmail { get; set; }

        // 抄送人邮箱
        [SugarColumn(IsJson = true, IsNullable = true, ColumnDataType = StaticConfig.CodeFirst_BigString)]
        public List<string> copyToEmails { get; set; }

        // 邮件主题
        [SugarColumn(Length = 500)]
        public string subject { get; set; }

        // 邮件 html 内容

        [SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString)]
        public string html { get; set; }

        // 进度信息
        public int index { get; set; }
        public int total { get; set; }

        // 生成成果
        [SugarColumn(IsNullable = true, ColumnDataType = StaticConfig.CodeFirst_BigString)]
        public string sendMessage { get; set; }
        public bool isSent { get; set; }

        // 尝试次数
        public int tryCount { get; set; }

        // 发送时间
        public DateTime sendDate { get; set; }

        // 发送格式
        public SendItemType sendItemType { get; set; }

        /// <summary>
        /// 内容 url
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string dataUrl { get; set; }

        [SugarColumn(IsJson = true, IsNullable = true,ColumnDataType = StaticConfig.CodeFirst_BigString)]
        // 待发附件
        public List<EmailAttachment> attachments { get; set; }

        public override string GetFilterString()
        {
            return base.GetFilterString() + senderName + senderEmail + receiverEmail + receiverEmail + subject + sendMessage;
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
