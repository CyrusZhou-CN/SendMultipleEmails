using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uamazing.Utils.Database.Attributes;

namespace UZonMailService.Models.LiteDB
{
    /// <summary>
    /// 发件箱
    /// </summary>
    [CollectionName("EmailBox")]
    public class Outbox : EmailBox
    {
        /// <summary>
        /// smtp 密码
        /// </summary>
        public string? SmtpPassword { get; set; }

        /// <summary>
        /// smtp 地址
        /// </summary>
        public string? SmtpAddress { get; set; }

        /// <summary>
        /// smtp 端口，默认 ssl 端口 465
        /// </summary>
        public int SmtpPort { get; set; } = 465;

        /// <summary>
        /// smtp 协议
        /// </summary>
        public string? SmtpProtocol { get; set; } = "https";

        /// <summary>
        /// 是否作为发件人  
        /// </summary>       
        public bool AsSender { get; set; } = true;

        /// <summary>
        /// 单日最大发件量
        /// 0 代表无限制
        /// </summary>
        public int MaxEmailsPerDay { get; set; } = 0;

        /// <summary>
        /// 总发件量
        /// </summary>
        public double SendCountTotal { get; set; }

        /// <summary>
        /// 当天发件数
        /// </summary>
        public int SentCountToday { get; set; }

        /// <summary>
        /// 记录单日发件的日期
        /// 系统自动修改
        /// </summary>
        public DateTime LastSendDate { get; set; }

        public override string GetFilterString()
        {
            return $"{base.GetFilterString()}{SmtpAddress}";
        }
    }
}
