using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uamazing.SME.Server.Models
{
    /// <summary>
    /// 发件箱
    /// </summary>
    public class Outbox : LinkingUserId
    {       
        /// <summary>
        /// smtp 密码
        /// </summary>
        public string SmtpPassword { get; set; }

        /// <summary>
        /// smtp 地址
        /// </summary>
        public string SmtpAddress { get; set; }

        /// <summary>
        /// smtp 端口，默认 ssl 端口 465
        /// </summary>
        public int SmtpPort { get; set; } = 465;

        public override string GetFilterString()
        {
            return $"{base.GetFilterString()}{SmtpAddress}";
        }
    }

    /// <summary>
    /// 发件箱设置
    /// </summary>
    public class SendBoxSetting
    {
        // 是否作为发件人
        [BsonField("asSender")]
        public bool AsSender { get; set; } = true;

        // 单日最大发件量
        [BsonField("maxEmailsPerDay")]
        public int MaxEmailsPerDay { get; set; } = 40;

        // 总发件量
        // 系统自动增加
        [BsonField("sendCountTotal")]
        public double SendCountTotal { get; set; }

        // 当天发件数
        [BsonField("sentCountToday")]
        public int SentCountToday { get; set; }

        // 记录单日发件的日期
        // 系统自动修改
        [BsonField("recordDate")]
        public string RecordDate { get; set; }
    }
}
