using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uamazing.SME.Server.Model
{
    /// <summary>
    /// 邮箱设置
    /// </summary>
    public class Setting : LinkingUserId
    {
        /// <summary>
        /// 设置类型
        /// </summary>
        public SettingType SettingType { get; set; }

        /// <summary>
        /// 时间间隔最大值
        /// 单位 s
        /// </summary>
        public double MaxSendInterval { get; set; }

        /// <summary>
        /// 发送时间间隔最小值
        /// 单位 s
        /// </summary>
        public double MinSendInterval { get; set; }

        /// <summary>
        /// 每日每个发件箱的最大发件量
        /// </summary>
        public int DailyMaxEmailsCount { get; set; }

        /// <summary>
        /// 系统独有设置
        /// 是否自动重发
        /// </summary>
        public bool IsAutoResend { get; set; }

        /// <summary>
        /// 生成默认配置
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static Setting GetDefaultSetting(SettingType settingType)
        {
            return new Setting()
            {
                SettingType = settingType,
                MinSendInterval = 5,
                MaxSendInterval = 15,
                IsAutoResend = true,
                DailyMaxEmailsCount = 40
            };
        }
    }

    public enum SettingType
    {
        /// <summary>
        /// 系统的
        /// </summary>
        System,

        /// <summary>
        /// 发件箱
        /// </summary>
        OutBox
    }
}
