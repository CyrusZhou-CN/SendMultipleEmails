using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Uamazing.SME.Server.Models;
using Uamazing.SME.Server.Services;
using Uamazing.Utils.Web.ResponseModel;
using Uamazing.Utils.Json;
using Uamazing.Utils.DotNETCore.Token;
using System.Collections.ObjectModel;
using Uamazing.Utils.Web.Extensions;
using Microsoft.Extensions.Options;

namespace Uamazing.SME.Server.Controllers
{
    /// <summary>
    /// 系统设置
    /// </summary>
    public class SettingController : CurdController<Setting>
    {
        private TokenParams _tokenParams;
        public SettingController(CurdService curdService, IOptions<TokenParams> options) : base(curdService)
        {
            _tokenParams = options.Value;
        }

        /// <summary>
        /// 更新发件间隔
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        [HttpPut("send-interval")]
        public async Task<ResponseResult<Setting>> SetSendInterval(double min,double max)
        {
            var (userId, _) = GetTokenInfo(_tokenParams);

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
            var (userId, _) = GetTokenInfo(_tokenParams);

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
            var (userId, _) = GetTokenInfo(_tokenParams);
            var setting = await CurdService.GetFirstOrDefault<Setting>(x => x.UserId == userId);
            return setting.ToSuccessResponse();
        }
    }
}
