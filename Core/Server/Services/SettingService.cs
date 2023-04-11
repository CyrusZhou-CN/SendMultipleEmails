using LiteDB;
using Uamazing.SME.Server.Models;

namespace Uamazing.SME.Server.Services
{
    /// <summary>
    /// 设置
    /// </summary>
    public class SettingService : CurdService
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
            var setting =await GetFirstOrDefault<Setting>(x => x.UserId == userId);
            if(setting == null)
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
