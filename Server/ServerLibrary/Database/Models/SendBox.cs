﻿using ServerLibrary.Database.Extensions;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary.Database.Models
{
    public class SendBox : ReceiveBox
    {
        public string password { get; set; }
        public string smtp { get; set; }

        /// <summary>
        /// 别名邮件地址（发件人地址）
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string aliasEmail {  get; set; }
        [SugarColumn(DefaultValue="587")]
        public int port { get; set; } = 587;
        [SugarColumn(DefaultValue = "true")]
        public bool enableSsl { get; set; } = true;
        /// <summary>
        /// 发件箱设置
        /// </summary>
        [SugarColumn(IsJson = true)]
        public SendBoxSetting settings { get; set; } = new SendBoxSetting();

        /// <summary>
        /// 递增发件量
        /// </summary>
        /// <returns>true:可以继续发件；false:发件已达到最大数量</returns>
        public bool IncreaseSentCount(ISqlSugarClient sqlDb, Setting globalSetting)
        {
            // 判断日期是否是今天，如果不是，则将当天发件数置 0
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            if (date != settings.recordDate)
            {
                settings.recordDate = date;
                settings.sentCountToday = 0;
            }

            settings.sentCountToday++;
            settings.sendCountTotal++;

            // 保存到数据库
            sqlDb.Update(this);

            int maxEmails = settings.maxEmailsPerDay > 0 ? settings.maxEmailsPerDay : globalSetting.maxEmailsPerDay;

            if (maxEmails < 1) return true;

            return settings.sendCountTotal <= maxEmails;
        }

        public override string GetFilterString()
        {
            return $"{base.GetFilterString()}{smtp}";
        }
    }

    /// <summary>
    /// 发件箱设置
    /// </summary>
    public class SendBoxSetting
    {
        // 是否作为发件人
        public bool asSender { get; set; } = true;

        // 单日最大发件量
        public int maxEmailsPerDay { get; set; } = 40;

        // 总发件量
        // 系统自动增加
        public double sendCountTotal { get; set; }

        // 当天发件数
        public int sentCountToday { get; set; }

        // 记录单日发件的日期
        // 系统自动修改
        public string recordDate { get; set; }
    }
}
