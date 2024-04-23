﻿using System.ComponentModel.DataAnnotations.Schema;
using UZonMailService.Models.SqlLite.Base;

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

        public int SmtpPort { get; set; }

        /// <summary>
        /// smtp 密码，需要加密保存
        /// smpt 密码 = 原始密码 > sha256 > aes
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 代理
        /// </summary>
        public string? Proxy { get; set; }

        /// <summary>
        /// 单日最大发送数量
        /// 为 0 时表示不限制
        /// </summary>
        public int MaxSendCountPerDay { get; set; }
    }
}
