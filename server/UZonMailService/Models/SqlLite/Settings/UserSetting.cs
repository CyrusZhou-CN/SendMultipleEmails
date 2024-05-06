using UZonMailService.Models.SqlLite.Base;

namespace UZonMailService.Models.SqlLite.Settings
{
    /// <summary>
    /// 用户设置
    /// </summary>
    public class UserSetting : GlobalUserSetting
    {
        /// <summary>
        /// 每日每个发件箱最大发送次数
        /// 为 0 时表示不限制
        /// </summary>
        public int MaxSendCountPerEmailDay { get; set; }
    }
}
