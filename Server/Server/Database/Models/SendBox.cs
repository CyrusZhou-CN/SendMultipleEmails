using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Models
{
    /// <summary>
    /// 发件箱
    /// </summary>
    public class SendBox : ReceiveBox
    {
        /// <summary>
        /// 密码
        /// </summary>
        [BsonField("password")]
        public string Password { get; set; }

        /// <summary>
        /// Smtp 设置
        /// </summary>
        [BsonField("smtp")]
        public string Smtp { get; set; }

        /// <summary>
        /// 发件箱设置
        /// </summary>
        [BsonField("settings")]
        public SendBoxSetting Settings { get; set; } = new SendBoxSetting();

        /// <summary>
        /// 递增发件量
        /// </summary>
        /// <returns>true:可以继续发件；false:发件已达到最大数量</returns>
        public bool IncreaseSentCount(LiteDBManager liteDb, UserSetting globalSetting)
        {
            // 判断日期是否是今天，如果不是，则将当天发件数置 0
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            if (date != Settings.RecordDate)
            {
                Settings.RecordDate = date;
                Settings.SentCountToday = 0;
            }

            Settings.SentCountToday++;
            Settings.SendCountTotal++;

            // 保存到数据库
            liteDb.Update(this);

            int maxEmails = Settings.MaxEmailsPerDay > 0 ? Settings.MaxEmailsPerDay : globalSetting.MaxEmailsPerDay;

            if (maxEmails < 1) return true;

            return Settings.SendCountTotal <= maxEmails;
        }

        public override string GetFilterString()
        {
            return $"{base.GetFilterString()}{Smtp}";
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
