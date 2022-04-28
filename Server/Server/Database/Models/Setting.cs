using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Models
{
    public class Setting:AutoObjectId
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [BsonField("userId")]
        public string UserId { get; set; }

        /// <summary>
        /// 时间间隔最大值
        /// </summary>
        [BsonField("sendInterval_max")]
        public double SendInterval_max { get; set; }

        /// <summary>
        /// 发送时间间隔最小值
        /// </summary>
        [BsonField("sendInterval_min")]
        public double SendInterval_min { get; set; }

        // 是否自动发送
        [BsonField("isAutoResend")]
        public bool IsAutoResend { get; set; }

        /// <summary>
        /// 图文混发
        /// </summary>
        [BsonField("sendWithImageAndHtml")]
        public bool SendWithImageAndHtml { get; set; }

        // 单日最大发件量
        [BsonField("maxEmailsPerDay")]
        public int MaxEmailsPerDay { get; set; }

        /// <summary>
        /// 生成默认配置
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static Setting DefaultSetting(string userId)
        {
            return new Setting()
            {
                UserId = userId,
                MaxEmailsPerDay = 40,
                IsAutoResend = true,
                SendInterval_max = 8,
                SendInterval_min = 3,
                SendWithImageAndHtml = false,
            };
        }
    }
}
