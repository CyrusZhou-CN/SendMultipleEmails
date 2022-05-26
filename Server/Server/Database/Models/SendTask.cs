using LiteDB;
using Newtonsoft.Json.Linq;
using Server.Http.Controller;
using Server.Http.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Models
{
    /// <summary>
    /// 发件任务
    /// 一次发件任务，会发送多个邮件
    /// </summary>
    public class SendTask:AutoObjectId
    {
        /// <summary>
        /// 所属用户的Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 发件箱
        /// 包含所有的发件邮箱
        /// 每个里面包含邮箱和名称，方便快速查询
        /// </summary>
        public List<EmailInfo> SenderEmails { get; set; }

        /// <summary>
        /// 收件箱
        /// 包含所有收件箱
        /// </summary>
        public List<EmailInfo> ReceiverEmails { get; set; }

        /// <summary>
        /// 全局抄送人邮箱
        /// </summary>
        public List<EmailInfo> GlobalCopyToEmails { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 主题名称
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 多个模板
        /// </summary>
        [BsonRef]
        public List<Template> Templates { get; set; } = new List<Template>();

        /// <summary>
        /// 从 Excel 表中读取的数据
        /// </summary>
        public JArray Data { get; set; }

        /// <summary>
        /// 发送状态
        /// </summary>
        public TaskStatus Status { get; set; }

        /// <summary>
        /// 临时数据:发送成功的数量
        /// </summary>
        public int SuccessCount { get; set; }

        /// <summary>
        /// 生成过滤字符串
        /// </summary>
        /// <returns></returns>
        public override string GetFilterString()
        {
            return base.GetFilterString() + Subject +string.Join(",",Templates.ConvertAll(t=>t.Name)) + SenderEmails.Count + ReceiverEmails.Count;
        }
    }
}
