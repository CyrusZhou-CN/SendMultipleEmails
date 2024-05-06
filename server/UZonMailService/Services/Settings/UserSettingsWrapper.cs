using Microsoft.EntityFrameworkCore;
using UZonMailService.Models.SqlLite;
using UZonMailService.Models.SqlLite.Settings;

namespace UZonMailService.Services.Settings
{
    /// <summary>
    /// 设置
    /// </summary>
    public class UserSettingsWrapper(GlobalUserSetting? globalSetting, UserSetting? userSetting)
    {
        /// <summary>
        /// 发件箱冷却时间
        /// </summary>
        public long OutboxCooldownMs
        {
            get
            {
                if (userSetting != null && userSetting.OutboxCooldownMs > 0)
                    return userSetting.OutboxCooldownMs;

                // 返回全局设置
                return globalSetting != null ? globalSetting.OutboxCooldownMs : 0;
            }
        }

        /// <summary>
        /// 获取每日最大发送次数
        /// </summary>
        /// <returns></returns>
        public int GetMaxSendCountPerEmailDay(int overrideValue)
        {
            if (overrideValue > 0) return overrideValue;
            if(userSetting != null)
                return userSetting.MaxSendCountPerEmailDay;
            return 0;
        }

        /// <summary>
        /// 创建用户设置
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<UserSettingsWrapper> CreateAsync(SqlContext db, int userId)
        {
            var userIds = new List<int>() { 1, userId };
            var settings = await db.GlobalUserSettings.Where(x => userIds.Contains(x.UserId)).ToListAsync();
            var global = settings.FirstOrDefault(x => x.UserId == 1);
            var user = settings.FirstOrDefault(x => x.UserId == userId);
            return new UserSettingsWrapper(global, user as UserSetting);
        }
    }
}
