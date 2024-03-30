using LiteDB;
using UZonMailService.Models.LiteDB;

namespace UZonMailService.Services.Litedb
{
    /// <summary>
    /// 设置
    /// </summary>
    public class SettingService : CRUDService
    {
        public SettingService(ILiteRepository liteRepository) : base(liteRepository)
        {
        }

        /// <summary>
        /// 创建用户设置
        /// 如果存在，则返回存在的设置
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Setting> CreateUserSetting(string userId)
        {
            var setting = await FirstOrDefault<Setting>(x => x.UserId == userId);
            if (setting == null)
            {
                setting = new Setting()
                {
                    UserId = userId,
                };
                await Create(setting);
            }
            return setting;
        }
    }
}
