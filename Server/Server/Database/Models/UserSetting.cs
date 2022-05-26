using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Models
{
    /// <summary>
    /// 用户设置
    /// </summary>
    public class UserSetting:AutoObjectId
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 时间间隔最大值
        /// </summary>
        public double SendInterval_max { get; set; }

        /// <summary>
        /// 发送时间间隔最小值
        /// </summary>
        public double SendInterval_min { get; set; }

        // 是否自动发送
        public bool IsAutoResend { get; set; }

        /// <summary>
        /// 图文混发
        /// </summary>
        public bool SendWithImageAndHtml { get; set; }

        // 单日最大发件量
        public int MaxEmailsPerDay { get; set; }

        /// <summary>
        /// 生成默认配置
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static UserSetting DefaultSetting(string userId)
        {
            return new UserSetting()
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
