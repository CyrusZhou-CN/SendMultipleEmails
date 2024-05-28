using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary.Database.Models
{
    /// <summary>
    /// 邮件附件
    /// </summary>
    public class EmailAttachment
    {

        /// <summary>
        /// 完整文件名
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string fileName { get; set; }

        /// <summary>
        /// 是否发送成功
        /// </summary>
        public bool isSent { get; set; }

        /// <summary>
        /// 发送失败原因
        /// </summary>
        public string reason { get; set; }
    }
}
