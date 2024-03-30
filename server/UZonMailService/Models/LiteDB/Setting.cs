using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UZonMailService.Models.LiteDB
{
    /// <summary>
    /// 邮箱设置
    /// </summary>
    public class Setting : LinkingUserId
    {
        /// <summary>
        /// 时间间隔最大值
        /// 单位 s
        /// </summary>
        public double MaxSendInterval { get; set; } = 120;

        /// <summary>
        /// 发送时间间隔最小值
        /// 单位 s
        /// </summary>
        public double MinSendInterval { get; set; } = 5;

        /// <summary>
        /// 每日每个发件箱的最大发件量
        /// 默认 40
        /// </summary>
        public int DailyMaxEmailsCount { get; set; } = 40;

        /// <summary>
        /// 系统独有设置
        /// 是否自动重发
        /// </summary>
        public bool IsAutoResend { get; set; } = true;
    }
}
