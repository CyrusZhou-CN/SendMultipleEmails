using System.ComponentModel.DataAnnotations.Schema;
using UZonMailService.Models.SqlLite.Base;
using UZonMailService.Models.SqlLite.EmailSending;
using UZonMailService.Models.SqlLite.EntityConfigs.Attributes;
using UZonMailService.Models.SqlLite.Settings;
using UZonMailService.Services.EmailSending.OutboxPool;

namespace UZonMailService.Models.SqlLite.Emails
{
    /// <summary>
    /// 发件箱
    /// </summary>
    public class Outbox : Inbox
    {
        public Outbox()
        {
            BoxType = EmailBoxType.Outbox;
        }

        /// <summary>
        /// SMTP 服务器地址
        /// </summary>
        public string SmtpHost { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int SmtpPort { get; set; }

        /// <summary>
        /// smtp 密码，需要加密保存
        /// smpt 密码 = 原始密码  > aes (sha256 作为 key)
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 是否启用 SSL
        /// </summary>
        public bool EnableSSL { get; set; } = true;

        /// <summary>
        /// 指定代理
        /// </summary>
        public SystemProxy? SystemProxy { get; set; }

        /// <summary>
        /// 单日最大发送数量
        /// 为 0 时表示不限制
        /// </summary>
        public int MaxSendCountPerDay { get; set; }

        /// <summary>
        /// 所属的发送组
        /// 与发送组是多对多的关系
        /// </summary>
        public List<SendingGroup>? SendingGroups { get; set; }

        /// <summary>
        /// 转成发件地址
        /// </summary>
        /// <param name="outboxCooldownMs"></param>
        /// <param name="maxSendCountPerDay"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public OutboxEmailAddress ToOutboxEmailAddress(long outboxCooldownMs,int maxSendCountPerDay,int groupId)
        {
            return new OutboxEmailAddress(outboxCooldownMs, maxSendCountPerDay)
            {
                // 对密码解密
                AuthPassword = Password,
                AuthUserName = Email,
                SmtpHost = SmtpHost,
                SmtpPort = SmtpPort,
                CreateDate = DateTime.Now,
                Email = Email,
                Name = Name,
                EnableSSL = EnableSSL,
                Id = Id,
                SendingGroupIds = [groupId]
            };
        }
    }
}
