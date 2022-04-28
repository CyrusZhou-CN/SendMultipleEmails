using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Models
{
    /// <summary>
    /// 邮件的附件
    /// </summary>
    public class EmailAttachment
    {
        [BsonField("fullName")]
        public string FullName { get; set; }

        /// <summary>
        /// 是否发送成功
        /// </summary>
        [BsonField("isSent")]
        public bool IsSent { get; set; }

        /// <summary>
        /// 发送失败原因
        /// </summary>
        [BsonField("reason")]
        public string Reason { get; set; }
    }
}
