using UZonMailService.Models.SqlLite.Base;

namespace UZonMailService.Models.SqlLite.Settings
{
    /// <summary>
    /// 系统设置
    /// </summary>
    public class GlobalUserSetting : SqlId
    {
        /// <summary>
        /// 用户 id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 发件箱冷却 ms 数据
        /// </summary>
        public long OutboxCooldownMs { get; set; }
    }
}
