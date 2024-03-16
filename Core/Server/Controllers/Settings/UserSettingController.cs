using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Uamazing.UZonEmail.Server.Models;
using Uamazing.UZonEmail.Server.Services;
using Uamazing.Utils.Web.ResponseModel;
using Uamazing.Utils.Json;
using System.Collections.ObjectModel;
using Uamazing.Utils.Web.Extensions;
using Microsoft.Extensions.Options;
using Uamazing.Utils.Web.Token;
using Uamazing.UZonEmail.Server.Services.Settings;

namespace Uamazing.UZonEmail.Server.Controllers.Settings
{
    /// <summary>
    /// 系统设置
    /// </summary>
    public class UserSettingController(CRUDService curdService, TokenService tokenService) : CurdController<Setting>(curdService)
    {
        /// <summary>
        /// 更新发件间隔
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        [HttpPut("send-interval")]
        public async Task<ResponseResult<Setting>> SetSendInterval(double min, double max)
        {
            var (userId, _) = tokenService.GetTokenInfo();
            
            // 找到设置
            var setting = await CurdService.UpdateOne(x => x.UserId == userId, new Setting()
            {
                MinSendInterval = min,
                MaxSendInterval = max
            }, new Utils.Database.LiteDB.UpdateOptions()
            {
                "MinSendInterval","MaxSendInterval"
            });

            return setting.ToSuccessResponse();
        }

        /// <summary>
        /// 修改自动重发
        /// </summary>
        /// <param name="isAutoResend"></param>
        /// <returns></returns>
        [HttpPut("auto-resend")]
        public async Task<ResponseResult<Setting>> ToggleAutoResend([FromBody] bool isAutoResend)
        {
            var (userId, _) = tokenService.GetTokenInfo();

            // 找到设置
            var setting = await CurdService.UpdateOne(x => x.UserId == userId, new Setting()
            {
                IsAutoResend = isAutoResend
            }, new Utils.Database.LiteDB.UpdateOptions()
            {
                "MinSendInterval","MaxSendInterval"
            });

            return setting.ToSuccessResponse();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseResult<Setting>> GetUserSetting()
        {
            var (userId, _) = tokenService.GetTokenInfo();
            var setting = await CurdService.FirstOrDefault<Setting>(x => x.UserId == userId);
            return setting.ToSuccessResponse();
        }
    }
}
